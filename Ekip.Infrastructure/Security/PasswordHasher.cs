using Ekip.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            throw new NotImplementedException();
        }
        public Task<bool> Verify(string hash, string password)
        {
            throw new NotImplementedException();
        }
    }
}
