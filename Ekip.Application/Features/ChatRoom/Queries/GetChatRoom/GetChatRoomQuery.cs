using Ekip.Application.DTOs.ChatRoom;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Features.ChatRoom.Queries.GetChatRoom
{
    public class GetChatRoomQuery : IRequest<ChatRoomListDto>
    {
        public Guid ChatRoomRef { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public List<Guid> Participants { get; set; }
        public int UnreadCount { get; set; }
    }
}
