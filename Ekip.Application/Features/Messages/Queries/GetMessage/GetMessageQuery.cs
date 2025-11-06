using Ekip.Application.DTOs;
using MediatR;

namespace Ekip.Application.Features.Messages.Queries.GetMessage
{
    public class GetMessageQuery : IRequest<List<MessageDto>>
    {
        public long ChatRoomId { get; set; }    
        public int Take { get; set; } = 50;
    }
}
