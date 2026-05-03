
namespace Ekip.Application.Interfaces
{
    public interface IChatService
    {
        Task SendMessageToGroupAsync(Guid chatRoomRef, object messageData);
    }
}
