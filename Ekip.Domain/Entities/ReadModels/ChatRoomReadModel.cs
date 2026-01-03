using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities.ReadModels
{
    public class ChatRoomReadModel
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public ChatRoomType ChatRoomType { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public Guid RequestRef { get; set; }
        public Guid CreatorRef { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public List<Guid> Participants { get; set; }
    }
}
