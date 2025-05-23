namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record JwtToken(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
