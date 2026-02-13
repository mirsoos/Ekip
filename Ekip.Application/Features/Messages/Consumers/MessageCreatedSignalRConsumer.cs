using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;

namespace Ekip.Application.Features.Messages.Consumers
{
    public class MessageCreatedSignalRConsumer : IConsumer<MessageCreatedEvent>
    {
        private readonly IChatService _chatService;

        public MessageCreatedSignalRConsumer(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task Consume(ConsumeContext<MessageCreatedEvent> context)
        {
            var message = context.Message;

            await _chatService.SendMessageToGroup(message.ChatRoomRef, new
            {
                Id = message.Id,
                SenderRef = message.SenderRef,
                Content = message.MessageContent,
                SentAt = message.SentAt,
                ReplyTo = message.ReplyToMessageRef,
                Type = message.Type
            });
        }
    }
}
