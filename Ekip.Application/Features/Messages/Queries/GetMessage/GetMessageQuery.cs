using Ekip.Application.DTOs.Chat;
using MediatR;

namespace Ekip.Application.Features.Messages.Queries.GetMessage
{
    public class GetMessageQuery : IRequest<List<MessageDto>>
    {
        public long ChatRoomRef { get; set; }    
        public int Take { get; set; } = 50;
    }
}
