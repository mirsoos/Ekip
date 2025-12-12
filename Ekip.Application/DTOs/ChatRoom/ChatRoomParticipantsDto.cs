
namespace Ekip.Application.DTOs.ChatRoom
{
    public class ChatRoomParticipantsDto
    {
        public long UserRef { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
    }
}
