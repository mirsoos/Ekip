using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;
using MassTransit;
using MediatR;
using Polly;

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

                    //await _publishEndpoint.Publish(new ProfileCreatedEvent
                    //{
                    //    Id = profile.Id,
                    //    UserRef = profile.UserRef,
                    //    Experience = profile.Experience,
                    //    Score = profile.Score
                    //});
                });
            }
            catch (ConcurrencyException)
            {
                throw new Exception("Username or Email already exists.");
            }




            //var existingUser = await _userWriteRepository.GetByUserNameAsync(request.UserName, cancellationToken);
            //if (existingUser != null)
            //    throw new Exception("This UserName already exists");

            //var existingEmail = await _userWriteRepository.GetByEmailAsync(request.Email, cancellationToken);
            //if (existingEmail != null)
            //    throw new Exception("This Email already exists");



               

            var userToken = _jwtTokenGenerator.GenerateToken(user.Id,user.Email,user.UserName);

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