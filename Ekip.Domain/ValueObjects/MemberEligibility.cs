
namespace Ekip.Domain.ValueObjects
{
    public sealed class MemberEligibility
    {
        public int Age { get; }
        public int Experience { get; }
        public double? Score { get; }
        public bool Gender { get; }

        public int Level => CalculateLevel(Experience);

        public MemberEligibility(int age, int experience, double? score, bool gender)
        {
            Age = age;
            Experience = experience;
            Score = score.HasValue ? score.Value : null;
            Gender = gender;
        }

        private static int CalculateLevel(int xp)
        {
            if (xp <= 0) return 1;
            return (xp / 500) + 1;
        }
    }
}
