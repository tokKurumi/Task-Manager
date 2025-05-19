namespace Task_Manager.Integrations;

public sealed record JwtOptions
{
    public string SecretKey { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public TimeSpan AccessTokenLifetime { get; init; } = default!;
}
