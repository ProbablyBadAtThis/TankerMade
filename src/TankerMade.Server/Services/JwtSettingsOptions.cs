namespace TankerMade.Server.Services;

public class JwtSettingsOptions
{
    public const string SectionName = "JwtSettings";
    public const int MinimumSecretLength = 32;

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
