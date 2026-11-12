using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Support
{
    public class TicketMessage : BaseEntity,ICreatedAuditable
    {
        public Guid TicketId { get; private set; }
        public Guid SenderId { get; private set; }
        public bool IsFromAdmin { get; private set; }
        public string MessageText { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        private readonly List<Guid> _attachmentAssetIds = [];
        public IReadOnlyCollection<Guid> AttachmentAssetIds => _attachmentAssetIds.AsReadOnly();
    }
}
