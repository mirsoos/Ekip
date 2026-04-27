
namespace Ekip.Domain.Entities.UserBehavior.Entities
{
    public class ScoreLedger 
    {
        public Guid Id { get; private set; }
        public DateTime CreateDate { get; private set; }
        public Guid RequestRef { get; private set; }
        public Guid TargetUserProfileRef { get; private set; }
        public Guid SourceUserProfileRef { get; private set; }
        public double ScoreGiven { get; private set; }
        public string? Comment { get; private set; }
        public ScoreLedger(Guid requestRef , Guid targetUserProfileRef , Guid sourceUserProfileRef , double scoreGiven , string? comment)
        {
            if (requestRef == Guid.Empty)
                throw new Exception("RequestRef cannot be Empty");
            if (targetUserProfileRef == Guid.Empty)
                throw new Exception("targetUserProfileRef cannot be Empty");
            if (sourceUserProfileRef == Guid.Empty)
                throw new Exception("sourceUserProfileRef cannot be Empty");
            if (scoreGiven < 1 || scoreGiven > 5)
                throw new Exception("Score must be between 1 and 5");

            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
            RequestRef = requestRef;
            TargetUserProfileRef = targetUserProfileRef;
            SourceUserProfileRef = sourceUserProfileRef;
            ScoreGiven = scoreGiven;
            Comment = comment;
        }

    }
}
