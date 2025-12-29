
namespace Ekip.Application.DTOs.ChatRoom
{
    public class ChatRoomListDto
    {
        public long ChatRoomRef { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public List<long> Participants { get; set; }
        public int? UnreadCount { get; set; }
    }
}
