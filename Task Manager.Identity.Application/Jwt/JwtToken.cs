namespace Task_Manager.Identity.Application.Jwt;

public sealed record JwtToken(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
