using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Messages.Consumers
{
    public class MessageCreatedConsumer : IConsumer<MessageCreatedEvent>
    {
        private readonly IMessageReadRepository _messageRead;
        public MessageCreatedConsumer(IMessageReadRepository messageRead)
        {
            _messageRead = messageRead;
        } 
        public async Task Consume(ConsumeContext<MessageCreatedEvent> context)
        {
            var message = context.Message;

            var mongoToPostgres = new MessageReadModel
            {
                Id = message.Id,
                ChatRoomRef = message.ChatRoomRef,
                MessageContent = message.MessageContent,
                SenderRef = message.SenderRef,
                Type = message.Type,
                SentAt = message.SentAt,
                ReplyToMessageRef = message.ReplyToMessageRef,
                IsDeleted = message.IsDeleted,
                IsEdited = message.IsEdited
            };

            await _messageRead.AddMessageAsync(mongoToPostgres, context.CancellationToken);
        }
    }
}
