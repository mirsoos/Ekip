using Ekip.Domain.Base;
using Ekip.Domain.Enums.AI;

namespace Ekip.Domain.Entities.AI
{
    public class AIConversation : BaseEntity , IAggregateRoot, IAuditableEntity,ISoftDeletable
    {
        public Guid UserId { get; private set; }
        public string? Title { get; private set; }
        public AIContextMode ContextMode { get; private set; }
        public int TotalTokensUsed { get; private set; } = 0;
        public bool IsMemoryExtracted { get; private set; } = false;
        public DateTime? LastMemoryExtractedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
    }
}
