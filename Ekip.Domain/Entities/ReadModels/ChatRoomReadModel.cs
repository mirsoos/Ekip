using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Domain.Entities.ReadModels
{
    public class ChatRoomReadModel
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public ChatRoomType ChatRoomType { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public Guid RequestRef { get; set; }
        public Guid CreatorRef { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public List<Guid> Participants { get; set; }
    }
}
