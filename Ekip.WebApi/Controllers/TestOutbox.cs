using Ekip.Application.Contracts.Events;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ekip.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestOutboxController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMongoClient _mongoClient;
        private readonly IConfiguration _configuration;

        public TestOutboxController(
            IPublishEndpoint publishEndpoint,
            IMongoClient mongoClient,
            IConfiguration configuration)
        {
            _publishEndpoint = publishEndpoint;
            _mongoClient = mongoClient;
            _configuration = configuration;
        }

        [HttpPost("test-outbox")]
        public async Task<IActionResult> TestOutbox()
        {
            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();

            try
            {
                await _publishEndpoint.Publish(new UserCreatedEvent
                {
                    UserRef = Guid.NewGuid(),
                    Email = "test@test.com",
                    UserName = "testuser"
                });

                await session.CommitTransactionAsync();

                return Ok(new { Message = "Published and Committed", SessionUsed = true });
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                return BadRequest(new { Error = ex.Message, SessionUsed = true });
            }
        }
    }

}
