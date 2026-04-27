using Ekip.Application.DTOs.User;
using MediatR;

namespace Ekip.Application.Features.Request.Qeuries.GetUserEkip
{
    public class GetUserEkipQuery : IRequest<List<MyEkipDto>>
    {
        public Guid ProfileRef { get; set; }
    }
}
