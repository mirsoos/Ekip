using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand,AuthenticationResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPublishEndpoint _publishEndpoint;

        public RegisterCommandHandler(IPasswordHasher passwordHasher , IUserReadRepository userReadRepository , IUserWriteRepository userWriteRepository , IJwtTokenGenerator jwtTokenGenerator,IPublishEndpoint publishEndpoint)
        {
            _passwordHasher = passwordHasher;
            _userReadRepository = userReadRepository;
            _userWriteRepository = userWriteRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Email))
                throw new Exception("user must have Email or UserName");

            if (await _userReadRepository.GetByUserNameAsync(request.UserName,cancellationToken) != null)
                throw new Exception("this userName already exist");

            if (await _userReadRepository.GetByEmailAsync(request.Email,cancellationToken) != null)
                throw new Exception("this email already exist");

            if (request.PhoneNumber.Length != 11)
                throw new Exception("PhoneNumber is Not Valid");

            var hashPassword =  _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName , request.LastName , request.UserName , request.Email , request.Gender,request.Age ,request.PhoneNumber);

            user.SetPasswordHash(hashPassword);


            await _userWriteRepository.AddAsync(user, cancellationToken);

            var userToken =  _jwtTokenGenerator.GenerateToken(user);

            await _publishEndpoint.Publish(new UserCreatedEvent
            {
                Id = user.Id,
                Age = user.Age,
                Email = user.Email,
                FirstName = user.FirstName,
                Gender = user.Gender,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Password = hashPassword
            });

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
