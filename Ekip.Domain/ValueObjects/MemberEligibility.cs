using Ekip.Domain.Base;
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Domain.ValueObjects
{
    public sealed class MemberEligibility : ValueObject
    {
        public int Age { get; }
        public int Experience { get; }
        public double? Score { get; }
        public GenderType Gender { get; }

        public int Level => CalculateLevel(Experience);

        public MemberEligibility(int age, int experience, double? score, GenderType gender)
        {
            Age = age;
            Experience = experience;
            Score = score;
            Gender = gender;
        }

        private static int CalculateLevel(int xp)
        {
            if (xp <= 0) return 1;
            return (xp / 500) + 1;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Age;
            yield return Experience;
            yield return Score ?? 0;
            yield return Gender;
        }

        private MemberEligibility() { }
    }
}
