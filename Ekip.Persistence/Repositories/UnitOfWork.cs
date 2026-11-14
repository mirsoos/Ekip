using Ekip.Application.Interfaces;
using Ekip.Domain.Results;
using Ekip.Persistence.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Persistence.Repositories;

internal sealed class UnitOfWork(
    EkipDbContext dbContext,
    IPublishEndpoint? publishEndpoint = null) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = dbContext.GetDomainEvents();

        if (domainEvents.Count > 0 && publishEndpoint is not null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await publishEndpoint.Publish(
                    domainEvent,
                    domainEvent.GetType(),
                    cancellationToken);
            }
        }

        var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.ClearDomainEvents();

        return affectedRows;
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        var attempt = 0;

        await strategy.ExecuteAsync(async ct =>
        {
            if (attempt++ > 0)
                dbContext.ChangeTracker.Clear();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                await action(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }, cancellationToken);
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        var attempt = 0;

        return await strategy.ExecuteAsync(async ct =>
        {
            if (attempt++ > 0)
                dbContext.ChangeTracker.Clear();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await action(ct);

                if (result is IResult { IsFailure: true })
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    dbContext.ClearDomainEvents();
                    dbContext.ChangeTracker.Clear();
                    return result;
                }

                await transaction.CommitAsync(ct);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }, cancellationToken);
    }
}