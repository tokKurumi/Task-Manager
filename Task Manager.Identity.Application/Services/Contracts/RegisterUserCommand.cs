namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
);

public sealed record RegisterUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
