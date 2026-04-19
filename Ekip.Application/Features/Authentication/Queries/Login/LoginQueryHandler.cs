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
        private readonly IRedisCacheService _redisCache;

        public LoginQueryHandler(IUserReadRepository userReadRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IRedisCacheService redisCache)
        {
            _userReadRepository = userReadRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _redisCache = redisCache;
        }

        public async Task<AuthenticationResult> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email))
                throw new Exception("cannot login with Empty UserName and Email");

            var user = await _userReadRepository.GetUserWithProfileByEmailOrUserNameAsync(request.UserName, request.Email,cancellationToken);

            if (user == null)
                throw new Exception("username/email or password Not Valid");

            var isPasswordValid =  _passwordHasher.Verify(request.Password , user.Password);

            if (!isPasswordValid)
                throw new Exception("username/email or password Not Valid");

            var token = _jwtTokenGenerator.GenerateToken(user.ProfileRef,user.PhoneNumber,user.UserName);

            return new AuthenticationResult
            {
                ProfileRef = user.ProfileRef,
                Email = user.Email,
                UserName = user.UserName,
                Token = token,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
