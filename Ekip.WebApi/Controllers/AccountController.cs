using Ekip.Application.DTOs.User;
using Ekip.Application.Features.Authentication.Commands.Register;
using Ekip.Application.Features.Authentication.Queries.Login;
using Ekip.Application.Features.Profile.Commands.SetUserAvatar;
using Ekip.Application.Features.Request.Commands.AssignToRequest;
using Ekip.Application.Features.Request.Commands.CreateRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AuthenticationResult>> Register(RegisterCommand register)
        {

            var result = await _mediator.Send(register);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResult>> Login(LoginQuery login)
        {
            if ((login.UserName == null || login.Email == null)&& login.Password == null)
                return BadRequest();

                var result = await _mediator.Send(login);

            return Ok(result);
        }

        [HttpPost("SetProfileAvatar")]
        public async Task<ActionResult<string>> SetProfileAvatar(SetUserAvatarCommand setUserAvatar)
        {
            if (setUserAvatar == null)
                return BadRequest();

            var result = await _mediator.Send(setUserAvatar);
            return Ok(result);
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
    }
}
