
namespace Ekip.Application.DTOs.UserBehavior
{
    public class SubmitScoreDto
    {
        public Guid RequestRef { get; set; }
        public Guid TargetUserProfileRef { get; set; }
        public Guid SourceUserProfileRef { get; set; }
        public double ScoreGiven { get; set; }
        public string? Comment { get; set; }
    }
}
