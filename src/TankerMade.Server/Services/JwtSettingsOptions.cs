namespace TankerMade.Server.Services;

public class JwtSettingsOptions
{
    public const string SectionName = "JwtSettings";
    public const int MinimumSecretLength = 32;

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// When greater than zero, token lifetime is measured in whole days (local-first default).
    /// Otherwise <see cref="ExpirationMinutes"/> is used.
    /// </summary>
    public int ExpirationDays { get; set; }

    public int ExpirationMinutes { get; set; } = 60;

    public DateTime GetExpiresAtUtc(DateTime fromUtc) =>
        ExpirationDays > 0
            ? fromUtc.AddDays(ExpirationDays)
            : fromUtc.AddMinutes(ExpirationMinutes);
}
