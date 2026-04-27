using Ekip.Application.DTOs.Chat;
using Ekip.Application.Features.Messages.Commands.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("SendMessage")]
        public async Task<ActionResult<MessageDto>> SendMessage(SendMessageCommand message)
        {
            if (message == null)
                return BadRequest();
            var result = await _mediator.Send(message);
            return Ok(result);
        }
    }
}
