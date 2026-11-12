namespace Ekip.Infrastructure.Caching.Redis;

public static class RedisSchemaKey
{
    private const string IdempotencyPrefix = "ekip:idempotency:";
    private const string OtpRegisterPrefix = "ekip:otp:register:";
    private const string OtpTwoFactorPrefix = "ekip:otp:2fa:";
    private const string OtpResetPasswordPrefix = "ekip:otp:reset_pw:";
    private const string OnlinePresencePrefix = "ekip:presence:online:";
    private const string AiDailyQuotaPrefix = "ekip:quota:ai_daily:";

    public static string Idempotency(Guid userId, Guid idempotencyKey)
        => $"{IdempotencyPrefix}{userId:N}:{idempotencyKey:N}";

    public static string OtpRegister(string phoneNumber)
        => $"{OtpRegisterPrefix}{phoneNumber.Trim()}";

    public static string OtpTwoFactor(Guid userId)
        => $"{OtpTwoFactorPrefix}{userId:N}";

    public static string OtpResetPassword(string phoneOrEmail)
        => $"{OtpResetPasswordPrefix}{phoneOrEmail.Trim().ToLowerInvariant()}";

    public static string OnlinePresence(Guid userId)
        => $"{OnlinePresencePrefix}{userId:N}";

    public static string AiDailyQuota(Guid userId, DateOnly utcDate)
        => $"{AiDailyQuotaPrefix}{userId:N}:{utcDate:yyyyMMdd}";
}