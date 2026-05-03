
namespace Ekip.Domain.Entities.UserBehavior.Entities
{
    public class ScoreLedger 
    {
        public Guid Id { get; private set; }
        public DateTime CreateDate { get; private set; }
        public Guid RequestRef { get; private set; }
        public Guid TargetUserRef { get; private set; }
        public Guid SourceUserRef { get; private set; }
        public double ScoreGiven { get; private set; }
        public string? Comment { get; private set; }
        public ScoreLedger(Guid requestRef , Guid targetUserRef , Guid sourceUserRef , double scoreGiven , string? comment)
        {
            if (requestRef == Guid.Empty)
                throw new Exception("RequestRef cannot be Empty");
            if (targetUserRef == Guid.Empty)
                throw new Exception("targetUserRef cannot be Empty");
            if (sourceUserRef == Guid.Empty)
                throw new Exception("sourceUserRef cannot be Empty");
            if (scoreGiven < 1 || scoreGiven > 5)
                throw new Exception("Score must be between 1 and 5");

            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
            RequestRef = requestRef;
            TargetUserRef = targetUserRef;
            SourceUserRef = sourceUserRef;
            ScoreGiven = scoreGiven;
            Comment = comment;
        }

    }
}
