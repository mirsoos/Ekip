using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IProfileWriteRepository _profileWriteRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPublishEndpoint _publishEndpoint;

        public RegisterCommandHandler(IPasswordHasher passwordHasher,IUserWriteRepository userWriteRepository,IJwtTokenGenerator jwtTokenGenerator,IPublishEndpoint publishEndpoint,IProfileWriteRepository profileWriteRepository)
        {
            _passwordHasher = passwordHasher;
            _userWriteRepository = userWriteRepository;
            _profileWriteRepository = profileWriteRepository;
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
            if (existingEmail != null)
                throw new Exception("This Email already exists");

            var hashPassword = _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName, request.LastName, request.UserName, request.Email, request.Gender, request.Age, request.PhoneNumber);
            user.SetPasswordHash(hashPassword);
            var profile = new ProfileEntity(user);

            user.SetProfileId(profile.Id);

            await _userWriteRepository.AddAsync(user, cancellationToken);

            await _profileWriteRepository.AddAsync(profile, cancellationToken);

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
                UserName = user.UserName,
                ProfileRef = user.ProfileRef,
                Password = hashPassword,
                CreateDate = user.CreateDate,
                IsDeleted = user.IsDeleted
            }, cancellationToken);

            await _publishEndpoint.Publish(new ProfileCreatedEvent
            {
                Id = profile.Id,
                AvatarUrl = profile.AvatarUrl,
                UserRef = profile.UserDetails.Id,
                Experience = profile.Experience,
                Score = profile.Score
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