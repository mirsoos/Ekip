using Ekip.Domain.Enums.Identity.Enums;
using System;

namespace Ekip.Domain.ValueObjects
{
    public class UserCredential
    {
        public string Value { get; private set; }         
        public AuthenticationType AuthenticationType { get; private set; }
        public bool IsVerified { get; private set; }

        public UserCredential(string value, AuthenticationType authenticationType, bool isVerified = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("value cannot be empty.");

            Value = value;
            AuthenticationType = authenticationType;
            IsVerified = isVerified;
        }

        public void Verify() => IsVerified = true;

            public override bool Equals(object? obj)
        {
            if (obj is not UserCredential other) return false;
            return Value == other.Value && AuthenticationType == other.AuthenticationType;
        }

        public override int GetHashCode() => HashCode.Combine(Value, AuthenticationType);
        private UserCredential() { }
    }
}
