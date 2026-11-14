using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.User;

public static class UserErrors
{
    public static readonly Error InvalidUsername = Error.Validation(
        "USER.INVALID_USERNAME",
        "Username must be between 3 and 50 characters and contain only allowed characters.");

    public static readonly Error InvalidEmail = Error.Validation(
        "USER.INVALID_EMAIL",
        "Email format is invalid or exceeds 256 characters.");

    public static readonly Error InvalidPhoneNumber = Error.Validation(
        "USER.INVALID_PHONE_NUMBER",
        "Phone number format is invalid or exceeds 20 characters.");

    public static readonly Error MissingCredentialsOrOAuth = Error.Validation(
        "USER.MISSING_CREDENTIALS_OR_OAUTH",
        "Account must have either password credentials or an OAuth provider.");

    public static readonly Error MissingContactMethod = Error.Validation(
        "USER.MISSING_CONTACT_METHOD",
        "Account must have at least an email address or a phone number.");

    public static readonly Error InvalidPasswordHash = Error.Validation(
        "USER.INVALID_PASSWORD_HASH",
        "Password hash cannot be empty or exceed 512 characters.");

    public static readonly Error InvalidReferralCode = Error.Validation(
        "USER.INVALID_REFERRAL_CODE",
        "Referral code must be between 4 and 20 characters.");

    public static readonly Error SelfReferralNotAllowed = Error.Validation(
        "USER.SELF_REFERRAL_NOT_ALLOWED",
        "A user cannot refer themselves.");

    public static readonly Error InvalidNationalIdHash = Error.Validation(
        "USER.INVALID_NATIONAL_ID_HASH",
        "National ID hash is required and cannot exceed 128 characters.");

    public static readonly Error AlreadyBanned = Error.Conflict(
        "USER.ALREADY_BANNED",
        "User account is already banned.");

    public static readonly Error AccountNotActive = Error.Forbidden(
        "USER.ACCOUNT_NOT_ACTIVE",
        "Operation is not allowed because the account is not active.");

    public static readonly Error InvalidOAuthProvider = Error.Validation(
        "USER.INVALID_OAUTH_PROVIDER",
        "OAuth provider name and provider user ID are required.");

    public static readonly Error OAuthProviderAlreadyLinked = Error.Conflict(
        "USER.OAUTH_PROVIDER_ALREADY_LINKED",
        "This OAuth provider is already linked to the account.");

    public static readonly Error OAuthProviderNotFound = Error.NotFound(
        "USER.OAUTH_PROVIDER_NOT_FOUND",
        "Specified OAuth provider is not linked to this account.");

    public static readonly Error CannotUnlinkLastLoginMethod = Error.Conflict(
        "USER.CANNOT_UNLINK_LAST_LOGIN_METHOD",
        "Cannot unlink the only remaining login method without a password set.");

    public static readonly Error InvalidFirstName = Error.Validation(
        "USER.INVALID_FIRST_NAME",
        "First name is required and cannot exceed 100 characters.");

    public static readonly Error InvalidLastName = Error.Validation(
        "USER.INVALID_LAST_NAME",
        "Last name is required and cannot exceed 100 characters.");

    public static readonly Error InvalidGender = Error.Validation(
        "USER.INVALID_GENDER",
        "Gender value is invalid.");

    public static readonly Error UnderageUser = Error.Validation(
        "USER.UNDERAGE",
        "User must be at least 18 years old.");

    public static readonly Error InvalidBirthDate = Error.Validation(
        "USER.INVALID_BIRTH_DATE",
        "Birth date is unrealistic.");

    public static readonly Error InvalidBio = Error.Validation(
        "USER.INVALID_BIO",
        "Bio cannot exceed 1000 characters.");

    public static readonly Error InvalidJobTitle = Error.Validation(
        "USER.INVALID_JOB_TITLE",
        "Job title cannot exceed 150 characters.");

    public static readonly Error InvalidMoodWidget = Error.Validation(
        "USER.INVALID_MOOD_WIDGET",
        "Mood widget must contain at least an emoji or description.");

    public static readonly Error InvalidSocialLink = Error.Validation(
        "USER.INVALID_SOCIAL_LINK",
        "Social link platform and valid URL are required.");

    public static readonly Error TooManySocialLinks = Error.Validation(
        "USER.TOO_MANY_SOCIAL_LINKS",
        "Cannot have more than 10 social links.");

    public static readonly Error InvalidInterest = Error.Validation(
        "USER.INVALID_INTEREST",
        "Each interest tag must be non-empty and at most 100 characters.");

    public static readonly Error TooManyInterests = Error.Validation(
        "USER.TOO_MANY_INTERESTS",
        "Cannot have more than 20 interests.");

    public static readonly Error SelfRelationNotAllowed = Error.Validation(
        "USER.SELF_RELATION_NOT_ALLOWED",
        "Cannot create a relation with yourself.");

    public static readonly Error InvalidRelationTransition = Error.Conflict(
        "USER.INVALID_RELATION_TRANSITION",
        "This relation state transition is not allowed.");

    public static readonly Error UnauthorizedRelationAction = Error.Forbidden(
        "USER.UNAUTHORIZED_RELATION_ACTION",
        "Only the allowed participant can perform this relation action.");

    public static readonly Error NationalIdAlreadyVerified = Error.Conflict(
    "USER.NATIONAL_ID_ALREADY_VERIFIED",
    "National ID has already been verified and cannot be changed.");
}