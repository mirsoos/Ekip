using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using MediatR;
using ChatRoomEntity = Ekip.Domain.Entities.Chat.Entites.ChatRoom;
using MassTransit;
using Ekip.Application.Contracts.Events;

namespace Ekip.Application.Features.ChatRoom.Commands.CreateChatRoom
{
    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, ChatRoomDetailsDto>
    {
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IRequestReadRepository _requestRead;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateChatRoomCommandHandler(IChatRoomWriteRepository chatRoomWrite , IRequestReadRepository requestRead , IPublishEndpoint publishEndpoint)
        {
            _chatRoomWrite = chatRoomWrite;
            _publishEndpoint = publishEndpoint;
            _requestRead = requestRead;
        }
        public async Task<ChatRoomDetailsDto> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {
            var newChatRoom = new ChatRoomEntity(request.Name,request.CreatorRef,request.ChatRoomType , request.RequestRef);

            var savedChatRoom = await _chatRoomWrite.AddChatRoomAsync(newChatRoom, cancellationToken);

            await _publishEndpoint.Publish(new ChatRoomCreatedEvent
            {
                AvatarUrl = savedChatRoom.AvatarUrl,
                ChatRoomRef = savedChatRoom.Id,
                ChatRoomType = savedChatRoom.ChatRoomType,
                CreateDate = savedChatRoom.CreateDate,
                CreatorRef = savedChatRoom.ChatRoomOwnerId,
                Name = savedChatRoom.Name,
                RequestRef = savedChatRoom.RequestRef,
                ChatRoomParticipants = savedChatRoom.Participants
            });

            var resultDto = new ChatRoomDetailsDto
            {
                AvatarUrl = savedChatRoom.AvatarUrl,
                ChatRoomRef = savedChatRoom.Id,
                ChatRoomType = savedChatRoom.ChatRoomType,
                CreateDate = savedChatRoom.CreateDate,
                CreatorRef = savedChatRoom.ChatRoomOwnerId,
                Name = savedChatRoom.Name,
                RequestRef = savedChatRoom.RequestRef,
                ChatRoomParticipants = savedChatRoom.Participants
            };

            return resultDto;
        }
    }
}
