using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException() : base() { }
        public DuplicateKeyException(string message) : base(message) { }
        public DuplicateKeyException(string message, Exception inner) : base(message, inner) { }
    }
}
