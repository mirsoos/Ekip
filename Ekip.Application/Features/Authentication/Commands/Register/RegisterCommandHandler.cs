using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPublishEndpoint _publishEndpoint;

        public RegisterCommandHandler(IPasswordHasher passwordHasher,IUserWriteRepository userWriteRepository,IJwtTokenGenerator jwtTokenGenerator,IPublishEndpoint publishEndpoint)
        {
            _passwordHasher = passwordHasher;
            _userWriteRepository = userWriteRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("User must have Email and UserName");

            if (request.PhoneNumber.Length != 11)
                throw new Exception("PhoneNumber is Not Valid");

            var existingUser = await _userWriteRepository.GetByUserNameAsync(request.UserName, cancellationToken);
            if (existingUser != null)
                throw new Exception("This UserName already exists");

            var existingEmail = await _userWriteRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingEmail)
                throw new Exception("This Email already exists");

            var hashPassword = _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName, request.LastName, request.UserName, request.Email, request.Gender, request.Age, request.PhoneNumber);
            user.SetPasswordHash(hashPassword);

            await _userWriteRepository.AddAsync(user, cancellationToken);

            var userToken = _jwtTokenGenerator.GenerateToken(user);

            await _publishEndpoint.Publish(new UserCreatedEvent
            {
                Id = user.Id,
                Age = user.Age,
                Email = user.Email,
                FirstName = user.FirstName,
                Gender = user.Gender,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
                // ❌ Password حذف شد (امنیت)
            }, cancellationToken);

            return new AuthenticationResult
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Token = userToken
            };
        }
    }
}