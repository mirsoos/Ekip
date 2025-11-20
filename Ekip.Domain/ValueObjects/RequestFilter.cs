using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    /// <summary>
    /// فیلتر های درخواستی کاربر برای درخواست اکیپ
    /// </summary>
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
                    if(!int.TryParse(value,out _))
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
    }
}