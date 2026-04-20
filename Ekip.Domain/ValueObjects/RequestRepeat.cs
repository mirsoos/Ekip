
using Ekip.Domain.Base;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    public class RequestRepeat : ValueObject
    {
        public bool IsRepeatable { get; private set; }
        public RequestRepeatType Type { get; private set; }

        public RequestRepeat(bool isRepeatable, RequestRepeatType type)
        {
            if (isRepeatable && type == RequestRepeatType.None)
                throw new ArgumentException("Repeatable request must have a repeat type.");

            if (!isRepeatable && type != RequestRepeatType.None)
                throw new ArgumentException("Non-repeatable request cannot have a repeat type.");

            IsRepeatable = isRepeatable;
            Type = type;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return IsRepeatable;
            yield return Type;
        }
        private RequestRepeat() { }
    }
}
