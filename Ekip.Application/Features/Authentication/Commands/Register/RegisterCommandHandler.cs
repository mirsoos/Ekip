using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.ValueObjects;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IPasswordHasher passwordHasher,IUserWriteRepository userWriteRepository,IJwtTokenGenerator jwtTokenGenerator , IEventPublisher eventPublisher , IUnitOfWork unitOfWork)
        {
            _passwordHasher = passwordHasher;
            _userWriteRepository = userWriteRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _eventPublisher = eventPublisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var hashPassword = _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName, request.LastName, request.UserName, email, request.Gender, request.Age, request.PhoneNumber , request.Bio);
            user.SetPasswordHash(hashPassword);

            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                await _userWriteRepository.AddAsync(user, innerCt);

                await _eventPublisher.Publish(new UserCreatedEvent
                {
                    UserRef = user.Id,
                    Age = user.Age,
                    Email = email.Value,
                    FirstName = user.FirstName,
                    Gender = user.Gender,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    CreateDate = user.CreateDate,
                    Experience = user.Profile.Experience,
                    Score = user.Profile.Score,
                    Bio = user.Profile.Bio
                }, innerCt);

            },cancellationToken);
               
            var userToken = _jwtTokenGenerator.GenerateToken(user.Id, email.Value, user.UserName);
            return new AuthenticationResult
            {
                Token = userToken
            };           
        }
    }
}