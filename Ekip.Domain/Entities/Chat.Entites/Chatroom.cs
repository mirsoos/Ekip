using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Domain.Entities.Chat.Entites
{
    public class ChatRoom : BaseEntity
    {
        public string Name { get; private set; }
        public ChatRoomType ChatRoomType { get; private set; }
        public Guid Creator { get; private set; }
        private List<Guid> _participants;
        public IReadOnlyCollection<Guid> Participants => _participants.AsReadOnly();
        public Guid RequestRef { get; private set; }
        public string? AvatarUrl { get; private set; }
        public ChatRoom(string name, Guid creator, ChatRoomType type, Guid requestRef)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Chatroom Must Have a Name");
            if (requestRef == Guid.Empty)
                throw new Exception("ChatRoom needs a valid RequestRef to be Created");
            RequestRef = requestRef;
            Name = name;
            ChatRoomType = type;
            Creator = creator;
            _participants = new List<Guid>() { creator };
        }

        public Message CreateMessage(Guid senderId, string content, Guid? replyTo = null)
        {
            if (!_participants.Contains(senderId))
                throw new Exception("You are not a member of this chatroom and cannot send messages.");

            return new Message(this.Id, senderId, content, replyTo);
        }

        public void AddParticipant(Guid userId)
        {
            if(!_participants.Contains(userId))
                _participants.Add(userId);

        }

        public void RemoveParticipant(Guid userId) 
        {

            if(userId == Creator)
            {
                if(ChatRoomType == ChatRoomType.Group && Participants.Count > 1)
                {
                    Guid newChatRoomOwnerId = Participants.First(u => u != userId);
                    ChangeChatRoomOwner(newChatRoomOwnerId);
                }
            }
            _participants.Remove(userId); 
        }

        public void ChangeChatRoomOwner(Guid newChatRoomOwnerId)
        {
            if (!_participants.Contains(newChatRoomOwnerId))
                throw new Exception("new chatRoomOwner must be a Participant of this Chat");

            Creator = newChatRoomOwnerId;
        }

        public void ChangeAvatar(string NewAvatar)
        {
            AvatarUrl = NewAvatar;
        }

        private ChatRoom() 
        {
            _participants = new List<Guid>();
        }
    }
}
