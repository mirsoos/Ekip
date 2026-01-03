using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserReadRepository _userRead;
        public UserCreatedConsumer(IUserReadRepository userRead)
        {
            _userRead = userRead;
        }
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;

            var mongoToPostgres = new UserReadModel
            {
                Id = user.Id,
                ProfileRef = user.ProfileRef,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CreateDate = user.CreateDate,
                Age = user.Age,
                Email = user.Email,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                IsDeleted = user.IsDeleted,
                Password = user.Password,
                
            };
            await _userRead.AddUserAsync(mongoToPostgres, context.CancellationToken);
        }
    }
}
