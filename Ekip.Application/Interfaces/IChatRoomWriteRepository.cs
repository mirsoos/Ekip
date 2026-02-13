using Ekip.Domain.Entities.Chat.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Interfaces
{
    public interface IChatRoomWriteRepository
    {
        Task<ChatRoom> AddChatRoomAsync(ChatRoom chatRoom,CancellationToken cancellationToken);
        Task<ChatRoom?> GetByIdAsync(Guid chatroomRef , CancellationToken cancellationToken);
    }
}
