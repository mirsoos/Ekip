using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.Media;

public static class MediaErrors
{
    public static readonly Error InvalidUsageContext = Error.Validation(
        "MEDIA.INVALID_USAGE_CONTEXT",
        "Media usage context is invalid.");

    public static readonly Error InvalidStorageKey = Error.Validation(
        "MEDIA.INVALID_STORAGE_KEY",
        "Storage key is required and cannot exceed 500 characters.");

    public static readonly Error InvalidNsfwProbability = Error.Validation(
        "MEDIA.INVALID_NSFW_PROBABILITY",
        "NSFW probability must be between 0 and 1.");

    public static readonly Error InvalidErrorMessage = Error.Validation(
        "MEDIA.INVALID_ERROR_MESSAGE",
        "Scan error message cannot be empty or exceed 1000 characters.");

    public static readonly Error InvalidModerationTransition = Error.Conflict(
        "MEDIA.INVALID_MODERATION_TRANSITION",
        "This moderation transition is not allowed for the current media status.");
}