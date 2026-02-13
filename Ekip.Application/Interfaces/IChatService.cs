
namespace Ekip.Application.Interfaces
{
    public interface IChatService
    {
        Task SendMessageToGroup(Guid chatRoomId, object messageData);
    }
}
