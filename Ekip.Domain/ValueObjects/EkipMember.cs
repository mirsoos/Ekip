using Ekip.Domain.Base;

namespace Ekip.Domain.ValueObjects
{
    public sealed class EkipMember : ValueObject
    {
        public Guid UserRef { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? AvatarUrl { get; init; }

        public EkipMember(Guid userRef, string firstName, string lastName, string? avatarUrl)
        {
            UserRef = userRef;
            FirstName = firstName;
            LastName = lastName;
            AvatarUrl = avatarUrl;
        }

        public string FullName => $"{FirstName} {LastName}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserRef;
        }

        private EkipMember() { }

    }
}
