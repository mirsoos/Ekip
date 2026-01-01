using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomWriteRepository : IChatRoomWriteRepository
    {
        public Task<ChatRoom> AddChatRoomAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
