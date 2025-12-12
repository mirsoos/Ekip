using Ekip.Application.DTOs.ChatRoom;
using Ekip.Domain.Enums.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Contracts.Events
{
    public record ChatRoomCreatedEvent
    {
        public long ChatRoomRef { get; init; }
        public ChatRoomType ChatRoomType { get; init; }
        public string Name { get; init; }
        public string? AvatarUrl { get; init ; }
        public long RequestRef { get; init; }
        public long CreatorRef { get; init; }
        public List<long>? ChatRoomParticipants { get; init; }
        public DateTime CreateDate { get; init; }
    }
}
