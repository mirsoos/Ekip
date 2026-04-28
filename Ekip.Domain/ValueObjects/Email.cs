using System.Net.Mail;
using Ekip.Domain.Base;

namespace Ekip.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }
        public string Domain { get; }
        public string Normalized { get; }


        private Email(string value, string domain, string normalized)
        {
            Value = value;
            Domain = domain;
            Normalized = normalized;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));

            Validate(value);

            var trimmed = value.Trim();
            var parts = trimmed.Split('@');

            var local = parts[0];
            var domain = parts[1].ToLowerInvariant();

            var normalized = $"{local.ToLowerInvariant()}@{domain}";

            return new Email(trimmed, domain, normalized);
        }

        private static void Validate(string value)
        {
            try
            {
                _ = new MailAddress(value);
            }
            catch
            {
                throw new ArgumentException("Invalid email format", nameof(value));
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Normalized;
        }

        public override string ToString() => Value;
        private Email() { }

    }
}
