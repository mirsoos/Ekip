using Ekip.Application.DTOs.User;
using Ekip.Application.Features.Authentication.Commands.Register;
using Ekip.Application.Features.Authentication.Queries.Login;
using Ekip.Application.Features.Profile.Commands.SetUserAvatar;
using Ekip.Infrastructure.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    //[Authorize]
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

        [HttpPost("SetProfileAvatar")]
        public async Task<ActionResult<string>> SetProfileAvatar(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("file does not Recieved");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var profileId = Guid.Parse(userIdClaim);

            string avatarUrl;
            using (var stream = file.OpenReadStream())
            {
                avatarUrl = await _fileService.UploadImageAsync(stream, file.FileName, "Avatars", cancellationToken);
            }

            var command = new SetUserAvatarCommand
            {
                AvatarUrl = avatarUrl,
                ProfileRef = profileId
            };

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok( result );
            }
            catch (Exception)
            {
                await _fileService.DeleteFile(avatarUrl, cancellationToken);
                throw;
            }
        }
    }
}
