using Ekip.Application.DTOs.User;
using Ekip.Application.Features.Authentication.Commands.Register;
using Ekip.Application.Features.Authentication.Queries.Login;
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
        [HttpPost("DoRegister")]
        public async Task<ActionResult<AuthenticationResult>> DoRegister()
        {
            return null;
        }
        [HttpPost("DoLogin")]
        public async Task<ActionResult<AuthenticationResult>> DoLogin()
        {
            return null;
        }
    }
}
