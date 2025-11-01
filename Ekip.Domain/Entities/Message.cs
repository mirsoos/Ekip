using Ekip.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities
{
    public class Message
    {
        public long Id { get; private set; }
        public Chatroom ChatroomRef { get; private set; }
        public int SenderRef { get; private set; }
        public string MessageContent { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsEdited { get; private set; }
        public MessageType Type { get; private set; }

        public Message(Chatroom chatroomRef,int senderRef,string messageContent)
        {
            if(string.IsNullOrEmpty(messageContent))
                throw new ArgumentException("Message content cannot be empty.");
            ChatroomRef = chatroomRef;
            SenderRef = senderRef;
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
