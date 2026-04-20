using Ekip.Domain.Base;
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Domain.ValueObjects
{
    public class UserCredential : ValueObject
    {
        public string Value { get; private set; }
        public AuthenticationType AuthenticationType { get; private set; }
        public bool IsVerified { get; private set; }

        public UserCredential(string value, AuthenticationType authenticationType, bool isVerified = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.", nameof(value));

            Value = value;
            AuthenticationType = authenticationType;
            IsVerified = isVerified;
        }

        public void Verify() => IsVerified = true;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return AuthenticationType;
        }

        private UserCredential() { }
    }
}