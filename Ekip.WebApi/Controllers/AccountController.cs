using Ekip.Application.DTOs.User;
using Ekip.Application.Features.Authentication.Commands.Register;
using Ekip.Application.Features.Authentication.Queries.Login;
using Ekip.Infrastructure.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;
        public AccountController(IMediator mediator , IFileService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
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
    }
}
