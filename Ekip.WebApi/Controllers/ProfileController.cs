using Ekip.Application.Features.Authentication.Commands.UserVerification;
using Ekip.Infrastructure.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Ekip.WebApi.Controllers
{
    [Route("EkipApi/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;

        public ProfileController(IMediator mediator , IFileService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }
        [HttpPost]
        [Route("FaceVerification")]
        public async Task<ActionResult<string>> FaceVerification(IFormFile file ,CancellationToken cancellationToken)
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
                avatarUrl = await _fileService.UploadImageAsync(stream, file.FileName, "tempVerification", cancellationToken);
            }

            var command = new FaceVerificationCommand
            {
                CapturedPhotoUrl = avatarUrl,
                ProfileRef = profileId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
