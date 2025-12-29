using Ekip.Application.Interfaces;
using BCrypt.Net;

namespace Ekip.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool Verify(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(hash, password);
        }
    }
}
