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
        public Guid ChatRoomRef { get; init; }
        public ChatRoomType ChatRoomType { get; init; }
        public string Name { get; init; }
        public string? AvatarUrl { get; init ; }
        public Guid RequestRef { get; init; }
        public Guid CreatorRef { get; init; }
        public List<Guid>? ChatRoomParticipants { get; init; }
        public DateTime CreateDate { get; init; }
    }
}
