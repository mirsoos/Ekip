using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    public record RequestFilter
    {
        public string Value { get; }
        public RequestFilterType Type { get; }
        public RequestFilterKind Kind { get; }

        private readonly int? _parsedInt;
        private readonly double? _parsedDouble;
        private readonly GenderType? _parsedGender;

        public RequestFilter(string value, RequestFilterType type, RequestFilterKind kind)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "RequestFilter cannot be Null.");

            Value = value;
            Type = type;
            Kind = kind;

            switch (type)
            {
                case RequestFilterType.Age or RequestFilterType.Level:
                    if (!int.TryParse(value, out var iVal))
                        throw new ArgumentException($"RequestFilter {type} property is not Valid.");
                    _parsedInt = iVal;
                    break;

                case RequestFilterType.Score:
                    if (!double.TryParse(value, out var dVal))
                        throw new ArgumentException($"RequestFilter {type} property is not Valid.");
                    _parsedDouble = dVal;
                    break;

                case RequestFilterType.Gender:
                    if (kind != RequestFilterKind.Equal)
                        throw new ArgumentException($"RequestFilter {type} property support [Equal] only");

                    if (!Enum.TryParse<GenderType>(value,true ,out var gVal))
                        throw new ArgumentException($"RequestFilter {type} property is not Valid.");
                    _parsedGender = gVal;
                    break;
            }
        }

        public bool IsSatisfiedBy(MemberEligibility member)
        {
            return Type switch
            {
                RequestFilterType.Age => Evaluate(_parsedInt!.Value, member.Age),

                RequestFilterType.Level => Evaluate(_parsedInt!.Value, member.Level),

                RequestFilterType.Score => member.Score.HasValue
                                           ? Evaluate(_parsedDouble!.Value, member.Score.Value)
                                           : false,

                RequestFilterType.Gender => member.Gender == _parsedGender!.Value,

                _ => true
            };
        }
        private bool Evaluate(double expected, double actual)
        {
            return Kind switch
            {
                RequestFilterKind.Equal => Math.Abs(actual - expected) < 0.001,
                RequestFilterKind.GreaterThan => actual > expected,
                RequestFilterKind.LessThan => actual < expected,
                RequestFilterKind.GreaterOrEqual => actual >= expected || Math.Abs(actual - expected) < 0.001,
                RequestFilterKind.LessOrEqual => actual <= expected || Math.Abs(actual - expected) < 0.001,
                _ => false
            };
        }
    }
}