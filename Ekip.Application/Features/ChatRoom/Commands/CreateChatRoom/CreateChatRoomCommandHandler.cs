using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using MediatR;
using ChatRoomEntity = Ekip.Domain.Entities.Chat.Entites.ChatRoom;
using MassTransit;
using Ekip.Application.Contracts.Events;
using Polly;
using Ekip.Application.Constants;

namespace Ekip.Application.Features.ChatRoom.Commands.CreateChatRoom
{
    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, ChatRoomDetailsDto>
    {
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;

        public CreateChatRoomCommandHandler(IChatRoomWriteRepository chatRoomWrite , IPublishEndpoint publishEndpoint, IRedisCacheService redisCache)
        {
            _chatRoomWrite = chatRoomWrite;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
        }
        public async Task<ChatRoomDetailsDto> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {
            var policy = Policy.Handle<ConcurrencyException>().RetryAsync(3);

            ChatRoomEntity newChatRoom = null!;
            ChatRoomEntity savedChatRoom = null!;

            try
            {
                await policy.ExecuteAsync(async () => {
                    newChatRoom = new ChatRoomEntity(request.Name, request.CreatorRef, request.ChatRoomType, request.RequestRef);

                    savedChatRoom = await _chatRoomWrite.AddChatRoomAsync(newChatRoom, cancellationToken);

                    await _publishEndpoint.Publish(new ChatRoomCreatedEvent
                    {
                        AvatarUrl = savedChatRoom.AvatarUrl,
                        ChatRoomRef = savedChatRoom.Id,
                        ChatRoomType = savedChatRoom.ChatRoomType,
                        CreateDate = savedChatRoom.CreateDate,
                        CreatorRef = savedChatRoom.Creator,
                        Name = savedChatRoom.Name,
                        RequestRef = savedChatRoom.RequestRef,
                        ChatRoomParticipants = savedChatRoom.Participants.ToList()
                    });

                    await _redisCache.RemoveAsync(CacheKeySchema.ChatRoomKey(savedChatRoom.Id), cancellationToken);

                });
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
