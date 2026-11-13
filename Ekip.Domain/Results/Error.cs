namespace Ekip.Domain.Results;

public record Error
{
    public static readonly Error None = new();

    public static readonly Error NullValue = Validation(
        "GENERAL.NULL_VALUE",
        "A null value was provided.");

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public IReadOnlyDictionary<string, object>? Metadata { get; }

    private Error()
    {
        Code = string.Empty;
        Message = string.Empty;
        Type = ErrorType.Failure;
    }

    protected Error(
        string code,
        string message,
        ErrorType type,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Type = type;
        Metadata = metadata is { Count: > 0 }
            ? new Dictionary<string, object>(metadata, StringComparer.Ordinal)
            : null;
    }

    public Error WithMetadata(string key, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        var dict = Metadata is null
            ? new Dictionary<string, object>(1, StringComparer.Ordinal)
            : new Dictionary<string, object>(Metadata, StringComparer.Ordinal);

        dict[key] = value;
        return new Error(Code, Message, Type, dict);
    }

    public Error WithMetadata(IReadOnlyDictionary<string, object> metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (metadata.Count == 0)
            return this;

        var dict = Metadata is null
            ? new Dictionary<string, object>(metadata.Count, StringComparer.Ordinal)
            : new Dictionary<string, object>(Metadata, StringComparer.Ordinal);

        foreach (var (key, value) in metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            dict[key] = value;
        }

        return new Error(Code, Message, Type, dict);
    }

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public virtual bool Equals(Error? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (EqualityContract != other.EqualityContract) return false;

        return string.Equals(Code, other.Code, StringComparison.Ordinal)
            && string.Equals(Message, other.Message, StringComparison.Ordinal)
            && Type == other.Type
            && MetadataEquals(Metadata, other.Metadata);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Code, StringComparer.Ordinal);
        hash.Add(Message, StringComparer.Ordinal);
        hash.Add(Type);

        if (Metadata is not null)
        {
            var metadataHash = 0;
            foreach (var (key, value) in Metadata)
            {
                metadataHash ^= HashCode.Combine(
                    StringComparer.Ordinal.GetHashCode(key),
                    value);
            }
            hash.Add(metadataHash);
        }

        return hash.ToHashCode();
    }

    private static bool MetadataEquals(
        IReadOnlyDictionary<string, object>? left,
        IReadOnlyDictionary<string, object>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null || left.Count != right.Count) return false;

        foreach (var (key, value) in left)
        {
            if (!right.TryGetValue(key, out var otherValue) || !Equals(value, otherValue))
                return false;
        }

        return true;
    }
}