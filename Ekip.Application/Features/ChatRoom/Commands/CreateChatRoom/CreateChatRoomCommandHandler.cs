using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using MediatR;
using ChatRoomEntity = Ekip.Domain.Entities.Chat.Entites.ChatRoom;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Constants;

namespace Ekip.Application.Features.ChatRoom.Commands.CreateChatRoom
{
    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, ChatRoomDetailsDto>
    {
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        private readonly IUnitOfWork _unitOfWork;

        public CreateChatRoomCommandHandler(IChatRoomWriteRepository chatRoomWrite , IEventPublisher eventPublisher, IRedisCacheService redisCache, IUnitOfWork unitOfWork)
        {
            _chatRoomWrite = chatRoomWrite;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
            _unitOfWork = unitOfWork;
        }
        public async Task<ChatRoomDetailsDto> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {

            var newChatRoom = new ChatRoomEntity(request.Name, request.CreatorRef, request.ChatRoomType, request.RequestRef);
            ChatRoomEntity savedChatRoom = null!;
            await _unitOfWork.ExecuteAsync(async(innerCt) =>
            {
                savedChatRoom = await _chatRoomWrite.AddChatRoomAsync(newChatRoom, innerCt);

                await _eventPublisher.Publish(new ChatRoomCreatedEvent
                {
                    AvatarUrl = savedChatRoom.AvatarUrl,
                    ChatRoomRef = savedChatRoom.Id,
                    ChatRoomType = savedChatRoom.ChatRoomType,
                    CreateDate = savedChatRoom.CreateDate,
                    CreatorRef = savedChatRoom.Creator,
                    Name = savedChatRoom.Name,
                    RequestRef = savedChatRoom.RequestRef,
                    ChatRoomParticipants = savedChatRoom.Participants.ToList()
                }, innerCt);

                await _redisCache.RemoveAsync(CacheKeySchema.ChatRoomKey(savedChatRoom.Id), cancellationToken);

            },cancellationToken);

            var resultDto = new ChatRoomDetailsDto
            {
                AvatarUrl = savedChatRoom.AvatarUrl,
                ChatRoomRef = savedChatRoom.Id,
                ChatRoomType = savedChatRoom.ChatRoomType,
                CreateDate = savedChatRoom.CreateDate,
                CreatorRef = savedChatRoom.Creator,
                Name = savedChatRoom.Name,
                RequestRef = savedChatRoom.RequestRef,
                ChatRoomParticipants = savedChatRoom.Participants.ToList()
            };

            return resultDto;
        }
    }
}
