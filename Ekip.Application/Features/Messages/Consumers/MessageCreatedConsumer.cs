using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Messages.Consumers
{
    public class MessageCreatedConsumer : IConsumer<MessageCreatedEvent>
    {
        private readonly IMessageReadRepository _messageRead;
        private readonly IChatRoomReadRepository _chatRoomRead;
        public MessageCreatedConsumer(IMessageReadRepository messageRead,IChatRoomReadRepository chatRoomRead)
        {
            _messageRead = messageRead;
            _chatRoomRead = chatRoomRead;
        } 
        public async Task Consume(ConsumeContext<MessageCreatedEvent> context)
        {
            var message = context.Message;

            await _chatRoomRead.UpdateLastMessageAsync(message.ChatRoomRef,message.MessageContent,message.SentAt,context.CancellationToken);

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
                IsEdited = message.IsEdited,
                
            };

            await _messageRead.AddMessageAsync(mongoToPostgres, context.CancellationToken);
        }
    }
}
