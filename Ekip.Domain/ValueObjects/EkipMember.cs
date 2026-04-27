using Ekip.Domain.Base;

namespace Ekip.Domain.ValueObjects
{
    public sealed class EkipMember : ValueObject
    {
        public Guid ProfileRef { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? AvatarUrl { get; init; }

        public EkipMember(Guid profileRef, string firstName, string lastName, string? avatarUrl)
        {
            ProfileRef = profileRef;
            FirstName = firstName;
            LastName = lastName;
            AvatarUrl = avatarUrl;
        }

        public string FullName => $"{FirstName} {LastName}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProfileRef;
        }

        private EkipMember() { }

    }
}
