using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using MediatR;
using ChatRoomEntity = Ekip.Domain.Entities.Chat.Entites.ChatRoom;
using MassTransit;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Constants;

namespace Ekip.Application.Features.ChatRoom.Commands.CreateChatRoom
{
    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, ChatRoomDetailsDto>
    {
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;

        public CreateChatRoomCommandHandler(IChatRoomWriteRepository chatRoomWrite , IEventPublisher eventPublisher, IRedisCacheService redisCache)
        {
            _chatRoomWrite = chatRoomWrite;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
        }
        public async Task<ChatRoomDetailsDto> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {

            ChatRoomEntity savedChatRoom = null!;

            try
            {

                    var newChatRoom = new ChatRoomEntity(request.Name, request.CreatorRef, request.ChatRoomType, request.RequestRef);

                    savedChatRoom = await _chatRoomWrite.AddChatRoomAsync(newChatRoom, cancellationToken);

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
                    },cancellationToken);

                    await _redisCache.RemoveAsync(CacheKeySchema.ChatRoomKey(savedChatRoom.Id), cancellationToken);

            }
            catch(ConcurrencyException)
            {
                throw new Exception("Request already exists.");
            }

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
