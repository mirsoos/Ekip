using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using MassTransit;
using MediatR;
using Polly;
using System.Text.RegularExpressions;
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

            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("User must have Email and UserName");

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                throw new Exception("Invalid email format");

            if (request.PhoneNumber.Length != 11 )
                throw new Exception("PhoneNumber is Not Valid");

            var policy = Policy.Handle<ConcurrencyException>().RetryAsync(3);

            User user = null!;
            ProfileEntity profile = null!;


            try
            {
                await policy.ExecuteAsync(async () =>
                {
                    var hashPassword = _passwordHasher.Hash(request.Password);

                    user = new User(request.FirstName, request.LastName, request.UserName, request.Email, request.Gender, request.Age, request.PhoneNumber);
                    user.SetPasswordHash(hashPassword);
                    profile = new ProfileEntity(user.Id);

                    user.SetProfileId(profile.Id);

                    await _userWriteRepository.AddAsync(user, cancellationToken);

                    await _profileWriteRepository.AddAsync(profile, cancellationToken);

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
                        Password = user.PasswordHash,
                        CreateDate = user.CreateDate,
                        IsDeleted = user.IsDeleted,
                        Experience = profile.Experience,
                        UserRef = profile.UserRef,
                        Score = profile.Score
                    }, cancellationToken);

                });
            }
            catch (ConcurrencyException)
            {
                throw new Exception("Username or Email already exists.");
            }

            var userToken = _jwtTokenGenerator.GenerateToken(user.ProfileRef,user.Email,user.UserName);

            return new AuthenticationResult
            {
                ProfileRef = user.ProfileRef,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Token = userToken
            };
        }
    }
}