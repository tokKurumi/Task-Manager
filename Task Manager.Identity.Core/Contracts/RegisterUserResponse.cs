namespace Task_Manager.Identity.Core.Contracts;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
