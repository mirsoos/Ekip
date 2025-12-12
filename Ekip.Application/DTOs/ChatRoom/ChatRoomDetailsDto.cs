
using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Application.DTOs.ChatRoom
{
    public class ChatRoomDetailsDto
    {
        public long ChatRoomRef { get; set; }
        public ChatRoomType ChatRoomType { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public long RequestRef { get; set; }
        public long CreatorRef { get; set; }
        public List<long> ChatRoomParticipants { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
