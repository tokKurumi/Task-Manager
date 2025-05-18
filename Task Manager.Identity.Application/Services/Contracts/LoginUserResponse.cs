namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record LoginUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
