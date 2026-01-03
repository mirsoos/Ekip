using Ekip.Application.DTOs.ChatRoom;
using Ekip.Domain.Enums.Chat.Enums;
using MediatR;

namespace Ekip.Application.Features.ChatRoom.Commands.CreateChatRoom
{
    public class CreateChatRoomCommand : IRequest<ChatRoomDetailsDto>
    {
        public ChatRoomType ChatRoomType { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public Guid RequestRef { get; set; }
        public Guid CreatorRef { get; set; }
        public List<ChatRoomParticipantsDto> ChatRoomParticipants { get; set; }
    }
}
