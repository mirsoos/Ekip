using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.UserBehavior.Entities;
using MassTransit;

namespace Ekip.Application.Features.Gamification.Consumers
{
    public class ScoreSubmittedConsumer : IConsumer<ScoreSubmittedEvent>
    {
        private readonly IScoreLedgerWriteRepository _scoreLedgerWrite;
        public ScoreSubmittedConsumer(IScoreLedgerWriteRepository scoreLedgerWrite)
        {
            _scoreLedgerWrite = scoreLedgerWrite;
        }
        public async Task Consume(ConsumeContext<ScoreSubmittedEvent> context)
        {
            var message = context.Message;

            var toMongo = new ScoreLedger(message.RequestRef, message.TargetUserProfileRef, message.SourceUserProfileRef, message.ScoreGiven, message.Comment);

            await _scoreLedgerWrite.AddScoreAsync(toMongo,context.CancellationToken);
        }
    }
}
