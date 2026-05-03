
namespace Ekip.Application.DTOs.UserBehavior
{
    public class SubmitScoreDto
    {
        public Guid RequestRef { get; set; }
        public Guid TargetUserRef { get; set; }
        public Guid SourceUserRef { get; set; }
        public double ScoreGiven { get; set; }
        public string? Comment { get; set; }
    }
}
