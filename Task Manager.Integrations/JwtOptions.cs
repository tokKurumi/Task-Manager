namespace Task_Manager.Integrations;

public sealed record JwtOptions
{
    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public TimeSpan AccessTokenLifetime { get; set; } = default!;
}

public static class JwtIntegration
{
    public static string JwtSecretKeyEnvironment => $"{nameof(JwtIntegration)}__{nameof(JwtOptions.SecretKey)}";
    public static string JwtIssuerEnvironment => $"{nameof(JwtIntegration)}__{nameof(JwtOptions.Issuer)}";
    public static string JwtAudienceEnvironment => $"{nameof(JwtIntegration)}__{nameof(JwtOptions.Audience)}";
    public static string JwtAccessTokenLifetimeEnvironment => $"{nameof(JwtIntegration)}__{nameof(JwtOptions.AccessTokenLifetime)}";
}
