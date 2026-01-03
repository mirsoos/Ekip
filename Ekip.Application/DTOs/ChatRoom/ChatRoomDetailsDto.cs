
using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Application.DTOs.ChatRoom
{
    public class ChatRoomDetailsDto
    {
        public Guid ChatRoomRef { get; set; }
        public ChatRoomType ChatRoomType { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public Guid RequestRef { get; set; }
        public Guid CreatorRef { get; set; }
        public List<Guid> ChatRoomParticipants { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
