namespace Ekip.Application.Contracts;

public sealed record CursorPagedResult<T>(
    IReadOnlyList<T> Items,
    string? NextCursor)
{
    public bool HasMore => NextCursor is not null;

    public static CursorPagedResult<T> Empty { get; } = new([], null);

    public static CursorPagedResult<T> Create(
        IReadOnlyList<T> fetchedItems,
        int pageSize,
        Func<T, string> cursorSelector)
    {
        ArgumentNullException.ThrowIfNull(fetchedItems);
        ArgumentNullException.ThrowIfNull(cursorSelector);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        if (fetchedItems.Count == 0)
            return Empty;

        if (fetchedItems.Count <= pageSize)
            return new(fetchedItems, null);

        IReadOnlyList<T> pageItems = fetchedItems is List<T> list
            ? list.GetRange(0, pageSize)
            : [.. fetchedItems.Take(pageSize)];

        var nextCursor = cursorSelector(pageItems[^1]);
        return new(pageItems, nextCursor);
    }
}