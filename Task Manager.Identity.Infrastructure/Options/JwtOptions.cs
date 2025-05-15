namespace Task_Manager.Identity.Infrastructure.Options;

public sealed record JwtOptions(
    string SecretKey,
    string Issuer,
    string Audience
);
