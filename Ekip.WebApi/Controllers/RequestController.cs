using Ekip.Application.Features.Request.Commands.AssignToRequest;
using Ekip.Application.Features.Request.Commands.CreateRequest;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Ekip.Application.Features.Messages.Commands.SendMessage;
using Ekip.Application.DTOs.Chat;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateRequest")]
        public async Task<ActionResult> CreateRequest(CreateRequestCommand newRequest)
        {
            var result = await _mediator.Send(newRequest);
            return Ok(result);
        }

        [HttpPost("AssignToRequest")]
        public async Task<ActionResult> AssingToRequest(AssignToRequestCommand assign)
        {
            var result = await _mediator.Send(assign);
            return Ok(result);
        }

        [HttpPost("SendMessage")]
        public async Task<ActionResult<MessageDto>> SendMessage(SendMessageCommand message)
        {
            var result = await _mediator.Send(message);
            return Ok(result);
        }
    }
}
