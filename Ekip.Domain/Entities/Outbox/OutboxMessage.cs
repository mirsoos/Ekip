
using Ekip.Domain.Enums.OutBox;

namespace Ekip.Domain.Entities.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string MessageType { get; private set; } = null!;
        public string Payload { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; private set; } = null;
        public Status Status { get; private set; } = Status.Pending;
        public int RetryCount { get; private set; } = 0;
        public string? Error { get; private set; } = null;

        public OutboxMessage(string messageType , string payload)
        {
            if (string.IsNullOrWhiteSpace(messageType) || string.IsNullOrWhiteSpace(payload))
                throw new ArgumentNullException("Invalid outbox message");
            MessageType = messageType;
            Payload = payload;
        }

        private OutboxMessage() { }

    }
}
