using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Domain.Entities.Chat.Entites
{
    public class Message : BaseEntity
    {
        public Guid ChatRoomRef { get; private set; }
        public Guid SenderRef { get; private set; }
        public string MessageContent { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsEdited { get; private set; }
        private readonly List<Guid> _seenBy = new();
        public IReadOnlyCollection<Guid> SeenBy => _seenBy.AsReadOnly();
        public Guid? ReplyToMessageRef { get; private set; }
        public MessageType Type { get; private set; }

        public Message(Guid chatroomRef,Guid senderRef,string messageContent,Guid? replyToMessageRef)
        {
            if(string.IsNullOrEmpty(messageContent))
                throw new ArgumentException("Message content cannot be empty.");
            ChatRoomRef = chatroomRef;
            SenderRef = senderRef;
            MessageContent = messageContent;
            SentAt = DateTime.UtcNow;
            IsEdited = false;
            Type = MessageType.Normal;
            ReplyToMessageRef = replyToMessageRef;
            _seenBy.Add(senderRef);
        }
        public void Edit(string messageContent)
        {
            if (string.IsNullOrEmpty(messageContent))
                throw new ArgumentException("Message content cannot be empty.");

            MessageContent = messageContent;
            IsEdited = true;
        }

        public void Delete() => IsDeleted = true;
        public void Pin() => Type = MessageType.Pinned;
        public void UnPin() => Type = MessageType.Normal;

        public void MarkAsRead(Guid userRef)
        {
            if (!_seenBy.Contains(userRef))
            {
                _seenBy.Add(userRef);
            }
        }

        private Message() { }
    }
}
