using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    public record RequestFilter
    {
        public string Value { get;}
        public RequestFilterType Type { get;}
        public RequestFilterKind Kind { get;}

        public RequestFilter(string value, RequestFilterType type, RequestFilterKind kind)
        {

            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "Filter value cannot be Null");


            switch (type)
            {
                case RequestFilterType.Age:
                case RequestFilterType.Level:
                case RequestFilterType.Score:
                    if(!double.TryParse(value,out _))
                    {
                        throw new ArgumentException($"The value for a filter of type '{type}' must be a valid Int.");
                    }
                    break;
                case RequestFilterType.Gender:
                    if(kind != RequestFilterKind.Equal)
                    {
                        throw new ArgumentException($"The operation for a filter of type '{type}' must be 'Equal'.");
                    }
                    break;
            }

            Value = value;
            Type = type;
            Kind = kind;
        }


        public bool IsSatisfiedBy(MemberEligibility member)
        {
            return Type switch
            {
                RequestFilterType.Age => Compare(member.Age),
                RequestFilterType.Level => Compare(member.Level),
                RequestFilterType.Score => member.Score.HasValue
                                           ? Compare(member.Score.Value)
                                           : false,
                RequestFilterType.Gender => CompareGender(member.Gender),
                _ => true
            };
        }

        private bool Compare(int actual)
        {
            if (!int.TryParse(Value, out var expected)) return false;
            return Evaluate(actual, expected);
        }

        private bool Compare(double actual)
        {
            if (!double.TryParse(Value, out var expected)) return false;
            return Evaluate(actual, expected);
        }

        private bool Evaluate(double actual, double expected)
        {
            return Kind switch
            {
                RequestFilterKind.Equal => Math.Abs(actual - expected) < 0.001,
                RequestFilterKind.GreaterThan => actual > expected,
                RequestFilterKind.LessThan => actual < expected,
                RequestFilterKind.GreaterOrEqual => actual >= expected,
                RequestFilterKind.LessOrEqual => actual <= expected,
                _ => false
            };
        }

        private bool CompareGender(bool actual)
        {
            if (bool.TryParse(Value, out var expected))
            {
                return actual == expected;
            }
            return false;
        }

    }
}