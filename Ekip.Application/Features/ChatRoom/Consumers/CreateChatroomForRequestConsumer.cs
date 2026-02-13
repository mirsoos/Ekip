using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using ChatRoomEntity = Ekip.Domain.Entities.Chat.Entites.ChatRoom;
using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Application.Features.ChatRoom.Consumers
{
    public class CreateChatroomForRequestConsumer : IConsumer<RequestCreatedEvent>
    {
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateChatroomForRequestConsumer(IChatRoomWriteRepository chatRoomWrite, IPublishEndpoint publishEndpoint)
        {
            _chatRoomWrite = chatRoomWrite;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<RequestCreatedEvent> context)
        {
            var message = context.Message;

            var chatRoom = new ChatRoomEntity(message.Title , message.CreatorRef , ChatRoomType.Group ,message.RequestRef);

            await _chatRoomWrite.AddChatRoomAsync(chatRoom,context.CancellationToken);

            await _publishEndpoint.Publish(new ChatRoomCreatedEvent
            {
                RequestRef = chatRoom.RequestRef,
                Name = message.Title ,
                CreatorRef = message.CreatorRef,
                CreateDate = chatRoom.CreateDate,
                ChatRoomType = chatRoom.ChatRoomType,
                ChatRoomRef = chatRoom.Id,
                ChatRoomParticipants = chatRoom.Participants.ToList(),
                AvatarUrl = chatRoom.AvatarUrl

            });
        }
    }
}
