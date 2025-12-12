using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Domain.Entities.Chat.Entites
{
    public class Message : BaseEntitiy
    {
        public long ChatRoomRef { get; private set; }
        public long SenderRef { get; private set; }
        public string MessageContent { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsEdited { get; private set; }
        private readonly List<long> _seenBy = new();
        public IReadOnlyCollection<long> SeenBy => _seenBy.AsReadOnly();
        public long? ReplyToMessageRef { get; private set; }
        public MessageType Type { get; private set; }

        public Message(long chatroomRef,long senderRef,string messageContent,long? replyToMessageRef)
        {
            if(string.IsNullOrEmpty(messageContent))
                throw new ArgumentException("Message content cannot be empty.");
            ChatRoomRef = chatroomRef;
            SenderRef = senderRef;
            MessageContent = messageContent;
            SentAt = DateTime.UtcNow;
            IsDeleted = false;
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

        public void MarkAsRead(long userRef)
        {
            if (!_seenBy.Contains(userRef))
            {
                _seenBy.Add(userRef);
            }
        }

        private Message() { }
    }
}
