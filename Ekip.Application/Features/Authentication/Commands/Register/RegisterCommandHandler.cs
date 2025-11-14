using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand,AuthenticationResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterCommandHandler(IPasswordHasher passwordHasher , IUserReadRepository userReadRepository , IUserWriteRepository userWriteRepository , IJwtTokenGenerator jwtTokenGenerator)
        {
            _passwordHasher = passwordHasher;
            _userReadRepository = userReadRepository;
            _userWriteRepository = userWriteRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Email))
                throw new Exception("user must have Email or UserName");

            if (await _userReadRepository.GetByUserNameAsync(request.UserName) != null)
                throw new Exception("this userName already exist");

            if (await _userReadRepository.GetByEmailAsync(request.Email) != null)
                throw new Exception("this email already exist");

            var hashPassword =  _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName , request.LastName , request.UserName , request.Email , request.Gender);

            user.SetPasswordHash(hashPassword);


            await _userWriteRepository.AddAsync(user, cancellationToken);

            var userToken =  _jwtTokenGenerator.GenerateToken(user);

            return new AuthenticationResult 
                {
                UserId = user.ID,
                UserName = user.UserName,
                Email = user.Email,
                Token = userToken

            };
        }
    }
}
