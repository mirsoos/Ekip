using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Features.ChatRoom.Queries.GetChatRoom
{
    public class GetChatRoomQueryHandler : IRequestHandler<GetChatRoomQuery, ChatRoomListDto>
    {
        private readonly IChatRoomReadRepository _chatRoomRead;
        public GetChatRoomQueryHandler(IChatRoomReadRepository chatRoomRead)
        {
            _chatRoomRead = chatRoomRead;
        }

        public async Task<ChatRoomListDto> Handle(GetChatRoomQuery request, CancellationToken cancellationToken)
        {
            var ChatRoomDto = await _chatRoomRead.GetListByIdAsync(request.ChatRoomRef, cancellationToken);

            if (ChatRoomDto == null)
                throw new Exception($"ChatRoom with ID {request.ChatRoomRef} Not Found");

            return ChatRoomDto;
        }
    }
}
