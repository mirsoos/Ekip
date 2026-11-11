using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.AI
{
    public class AIMessage : BaseEntity,IAggregateRoot,ICreatedAuditable
    {
        public string Content { get; private set; }
        public bool SentByUser { get; private set; }
        public Guid AIConversationId { get; private set; }
        public int TokensUsed { get; private set; } = 0;
        public Guid IdempotencyKey { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        private readonly List<Guid> _suggestedEkipIds = [];
        public IReadOnlyCollection<Guid> SuggestedEkipIds => _suggestedEkipIds.AsReadOnly();
    }
}
