using Ekip.Domain.Enums;


namespace Ekip.Domain.Entities
{
    public class ChatRoom
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public ChatRoomType ChatRoomType { get; private set; }
        public DateTime CreateDate { get; private set; }
        public int ChatRoomOwnerId { get; private set; }
        public List<int> Participants { get; private set; }

        private readonly List<Message> _messages = new();
        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

        public ChatRoom(string name,int chatRoomOwnerId,ChatRoomType type)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Chatroom Must Have a Name");

            Name = name;
            ChatRoomType = type;
            ChatRoomOwnerId = chatRoomOwnerId;
            CreateDate = DateTime.UtcNow;
            Participants = new List<int>() { chatRoomOwnerId };
        }

        public void AddParticipant(int userId)
        {
            if(!Participants.Contains(userId))
                Participants.Add(userId);

        }

        public void RemoveParticipant(int userId) 
        {

            if(userId == this.ChatRoomOwnerId)
            {
                if(ChatRoomType == ChatRoomType.Group && Participants.Count > 1)
                {
                    int newChatRoomOwnerId = Participants.First(u => u != userId);
                    ChangeChatRoomOwner(newChatRoomOwnerId);
                }
            }
            Participants.Remove(userId); 
        }

        public void ChangeChatRoomOwner(int newChatRoomOwnerId)
        {
            if (!Participants.Contains(newChatRoomOwnerId))
                throw new Exception("new chatRoomOwner must be a Participant of this Chat");

            ChatRoomOwnerId = newChatRoomOwnerId;
        }

        public Message SendMessage(int senderId , string messageContent)
        {
            if (!Participants.Contains(senderId))
                throw new ArgumentException("User is not participant of this chat");

            var message = new Message(this, senderId, messageContent);
            
                _messages.Add(message);

                return message;
            
        }

        private ChatRoom() { }
    }
}
