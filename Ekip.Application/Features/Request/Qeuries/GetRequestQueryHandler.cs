using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Request.Qeuries
{
    public class GetRequestQueryHandler : IRequestHandler<GetRequestQuery, RequestDetailsDto>
    {
        private readonly IRequestReadRepository _requestRead;
        public GetRequestQueryHandler(IRequestReadRepository requestRead)
        {
            _requestRead = requestRead;
        }

        public async Task<RequestDetailsDto> Handle(GetRequestQuery request, CancellationToken cancellationToken)
        {
            var requestDetailsDto = await _requestRead.GetRequestByIdAsync(request.RequestRef,cancellationToken);

            if (requestDetailsDto == null)
                throw new Exception($"Request with RequestId '{request.RequestRef}' Not Found");

            return requestDetailsDto;
        }
    }
}
