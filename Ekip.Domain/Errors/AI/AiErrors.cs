using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.AI;

public static class AiErrors
{
    public static readonly Error InvalidVectorDimensions = Error.Validation(
        "AI.INVALID_VECTOR_DIMENSIONS",
        "Identity vector must contain exactly 1536 dimensions.");

    public static readonly Error InvalidVectorValues = Error.Validation(
        "AI.INVALID_VECTOR_VALUES",
        "Identity vector must contain only finite floating-point values.");

    public static readonly Error InvalidPersonaSummary = Error.Validation(
        "AI.INVALID_PERSONA_SUMMARY",
        "AI persona summary cannot exceed 4000 characters.");

    public static readonly Error InvalidBirthYear = Error.Validation(
        "AI.INVALID_BIRTH_YEAR",
        "Birth year is invalid for an adult user.");

    public static readonly Error InvalidLevel = Error.Validation(
        "AI.INVALID_LEVEL",
        "User level cannot be less than 1.");
}