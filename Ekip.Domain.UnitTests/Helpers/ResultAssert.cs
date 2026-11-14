using Ekip.Domain.Base;
using Ekip.Domain.Results;

namespace Ekip.Domain.UnitTests.Helpers;

internal static class ResultAssert
{
    public static void Success(Result result) =>
        Assert.True(result.IsSuccess, result.IsFailure ? $"Expected success, got {result.Error.Code}: {result.Error.Message}" : string.Empty);

    public static TValue Success<TValue>(Result<TValue> result)
    {
        Assert.True(result.IsSuccess, result.IsFailure ? $"Expected success, got {result.Error.Code}: {result.Error.Message}" : string.Empty);
        return result.Value;
    }

    public static void Failure(IResult result, Error expectedError)
    {
        Assert.True(result.IsFailure, "Expected failure, but result succeeded.");
        Assert.Equal(expectedError, result.Error);
    }

    public static TEvent HasDomainEvent<TEvent>(Entity entity)
        where TEvent : class, IDomainEvent
    {
        var domainEvent = entity.DomainEvents.OfType<TEvent>().FirstOrDefault();
        Assert.NotNull(domainEvent);
        return domainEvent;
    }
}