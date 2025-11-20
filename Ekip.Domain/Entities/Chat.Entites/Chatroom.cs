using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Domain.Entities.Chat.Entites
{
    public class ChatRoom : BaseEntitiy
    {
        public string Name { get; private set; }
        public ChatRoomType ChatRoomType { get; private set; }
        public int ChatRoomOwnerId { get; private set; }
        public List<int> Participants { get; private set; }

        private readonly List<Message> _messages = new();
        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
        public long RequestRef { get; private set; }
        public Request Request { get; private set; }
        public ChatRoom(string name,int chatRoomOwnerId,ChatRoomType type,Request request)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Chatroom Must Have a Name");
            if (request == null)
                throw new Exception("ChatRoom needs a Request to be Created");
            Request = request;
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

            if(userId == ChatRoomOwnerId)
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
