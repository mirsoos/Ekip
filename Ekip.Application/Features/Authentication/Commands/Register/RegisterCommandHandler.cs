using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.ValueObjects;
using MassTransit;
using MediatR;
using Polly;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;

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
            var email = Email.Create(request.Email);

                var hashPassword = _passwordHasher.Hash(request.Password);

                var user = new User(request.FirstName, request.LastName, request.UserName, email, request.Gender, request.Age, request.PhoneNumber);
                user.SetPasswordHash(hashPassword);
                var profile = new ProfileEntity(user.Id);

                user.SetProfileId(profile.Id);
                profile.SetBio(request.Bio);

                await _userWriteRepository.AddAsync(user, cancellationToken);
                    
                await _profileWriteRepository.AddAsync(profile, cancellationToken);

                await _publishEndpoint.Publish(new UserCreatedEvent
                {
                    Id = user.Id,
                    Age = user.Age,
                    Email = email.Value,
                    FirstName = user.FirstName,
                    Gender = user.Gender,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    ProfileRef = user.ProfileRef,
                    CreateDate = user.CreateDate,
                    IsDeleted = user.IsDeleted,
                    Experience = profile.Experience,
                    UserRef = profile.UserRef,
                    Score = profile.Score,
                    Bio = profile.Bio
                }, cancellationToken);


            var userToken = _jwtTokenGenerator.GenerateToken(user.ProfileRef,email.Value,user.UserName);

            return new AuthenticationResult
            {
                ProfileRef = user.ProfileRef,
                Token = userToken
            };
        }
    }
}