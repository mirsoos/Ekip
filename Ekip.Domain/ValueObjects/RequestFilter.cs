using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    /// <summary>
    /// فیلتر های درخواستی کاربر برای درخواست اکیپ
    /// </summary>
    public class RequestFilter
    {
        public RequestFilter(string value, RequestFilterType type, RequestFilterKind kind)
        {
            Value = value;
            Type = type;
            Kind = kind;
        }

        public string Value { get; set; }
        public RequestFilterType Type { get; set; }
        public RequestFilterKind Kind { get; set; }
    }
}