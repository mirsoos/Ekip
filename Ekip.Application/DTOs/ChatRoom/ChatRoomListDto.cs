
namespace Ekip.Application.DTOs.ChatRoom
{
    public class ChatRoomListDto
    {
        public Guid ChatRoomRef { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public List<Guid> Participants { get; set; }
        public int? UnreadCount { get; set; }
    }
}
