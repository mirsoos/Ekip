using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Requests.Entities;
using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Domain.Entities.Chat.Entites
{
    public class ChatRoom : BaseEntitiy
    {
        public string Name { get; private set; }
        public ChatRoomType ChatRoomType { get; private set; }
        public long ChatRoomOwnerId { get; private set; }
        public List<long> Participants { get; private set; }
        public long RequestRef { get; private set; }
        public string AvatarUrl { get; set; }
        public ChatRoom(string name, long chatRoomOwnerId, ChatRoomType type, long requestRef)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Chatroom Must Have a Name");
            if (requestRef <= 0)
                throw new Exception("ChatRoom needs a valid RequestRef to be Created");
            RequestRef = requestRef;
            Name = name;
            ChatRoomType = type;
            ChatRoomOwnerId = chatRoomOwnerId;
            CreateDate = DateTime.UtcNow;
            Participants = new List<long>() { chatRoomOwnerId };
        }

        public void AddParticipant(long userId)
        {
            if(!Participants.Contains(userId))
                Participants.Add(userId);

        }

        public void RemoveParticipant(long userId) 
        {

            if(userId == ChatRoomOwnerId)
            {
                if(ChatRoomType == ChatRoomType.Group && Participants.Count > 1)
                {
                    long newChatRoomOwnerId = Participants.First(u => u != userId);
                    ChangeChatRoomOwner(newChatRoomOwnerId);
                }
            }
            Participants.Remove(userId); 
        }

        public void ChangeChatRoomOwner(long newChatRoomOwnerId)
        {
            if (!Participants.Contains(newChatRoomOwnerId))
                throw new Exception("new chatRoomOwner must be a Participant of this Chat");

            ChatRoomOwnerId = newChatRoomOwnerId;
        }

        private ChatRoom() { }
    }
}
