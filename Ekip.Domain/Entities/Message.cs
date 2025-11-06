using Ekip.Domain.Enums;


namespace Ekip.Domain.Entities
{
    public class Message
    {
        public long Id { get; private set; }
        public ChatRoom ChatRoom { get; private set; }
        public int SenderId { get; private set; }
        public string MessageContent { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsEdited { get; private set; }
        public MessageType Type { get; private set; }

        public Message(ChatRoom chatroom,int senderId,string messageContent)
        {
            if(string.IsNullOrEmpty(messageContent))
                throw new ArgumentException("Message content cannot be empty.");
            ChatRoom = chatroom;
            SenderId = senderId;
            MessageContent = messageContent;
            SentAt = DateTime.UtcNow;
            IsDeleted = false;
            IsEdited = false;
            Type = MessageType.Normal;
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
    }
}
