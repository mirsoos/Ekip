using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Authentication.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthenticationResult>
    {

        private readonly IUserReadRepository _userReadRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginQueryHandler(IUserReadRepository userReadRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userReadRepository = userReadRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email))
                throw new Exception("cannot login with Empty UserName and Email");

            var user = await _userReadRepository.GetByUserNameOrEmailAsync(request.UserName, request.Email);

            if (user == null)
                throw new Exception("username/email or password Not Valid");

            var isPasswordValid = await _passwordHasher.Verify(user.PasswordHash ,request.Password);

            if (!isPasswordValid)
                throw new Exception("username/email or password Not Valid");

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthenticationResult
            {
                UserId = user.ID,
                Email = user.Email,
                UserName = user.UserName,
                Token = token
            };
        }
    }
}
