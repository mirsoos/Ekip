using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.ChatRoom.Consumers
{
    public class ChatRoomCreatedConsumer : IConsumer<ChatRoomCreatedEvent>
    {
        private readonly IChatRoomReadRepository _chatRoomRead;
        public ChatRoomCreatedConsumer(IChatRoomReadRepository chatRoomRead)
        {
            _chatRoomRead = chatRoomRead;
        }
        public async Task Consume(ConsumeContext<ChatRoomCreatedEvent> context)
        {
            var chatRoom = context.Message;

            var mongoToPostgres = new ChatRoomReadModel
            {
                Id = chatRoom.ChatRoomRef,
                Name = chatRoom.Name,
                CreatorRef = chatRoom.CreatorRef,
                RequestRef = chatRoom.RequestRef,
                Participants = chatRoom.ChatRoomParticipants,
                CreateDate = chatRoom.CreateDate,
                ChatRoomType = chatRoom.ChatRoomType,
                AvatarUrl = chatRoom.AvatarUrl
            };

            await _chatRoomRead.AddChatRoomAsync(mongoToPostgres, context.CancellationToken);
        }
    }
}
