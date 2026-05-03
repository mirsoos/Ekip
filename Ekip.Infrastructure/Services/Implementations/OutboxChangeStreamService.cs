using System.Text.Json;
using Ekip.Domain.Entities.Outbox;
using Ekip.Domain.Enums.OutBox;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public class OutboxChangeStreamService : BackgroundService
{
    private readonly IMongoCollection<OutboxMessage> _collection;
    private readonly IBus _bus;
    private readonly ILogger<OutboxChangeStreamService> _logger;
    private const int BatchSize = 30;

    public OutboxChangeStreamService(IBus bus , IMongoDatabase db , ILogger<OutboxChangeStreamService> logger)
    {
        _bus = bus;
        _collection = db.GetCollection<OutboxMessage>("OutboxMessages");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await ProcessBatch(ct);

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<OutboxMessage>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert);

        using var cursor = await _collection.WatchAsync(pipeline, cancellationToken: ct);

        await cursor.ForEachAsync(async _ =>
        {
            await ProcessBatch(ct);
        }, ct);
    }

    private async Task ProcessBatch(CancellationToken ct)
    {
        var pending = await _collection.Find(x => x.Status == Status.Pending)
            .SortBy(x => x.CreatedAt)
            .Limit(BatchSize)
            .ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var msg in pending)
        {
            try
            {
                var type = Type.GetType(msg.MessageType);
                var data = JsonSerializer.Deserialize(msg.Payload, type!);
                await _bus.Publish(data!, type!, ct);

                var update = Builders<OutboxMessage>.Update
                    .Set(x => x.Status, Status.Sent)
                    .Set(x => x.ProcessedAt, DateTime.UtcNow)
                    .Set(x => x.Error, null);

                await _collection.UpdateOneAsync(x => x.Id == msg.Id, update, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                var failUpdate = Builders<OutboxMessage>.Update
                    .Inc(x => x.RetryCount, 1)
                    .Set(x => x.Error, ex.Message);

                await _collection.UpdateOneAsync(x => x.Id == msg.Id, failUpdate, cancellationToken: ct);
                _logger.LogError(ex, "Failed outbox message {Id}", msg.Id);
            }
        }
    }
}