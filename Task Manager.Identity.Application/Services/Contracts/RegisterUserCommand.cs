using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
);

public sealed record RegisterUserResponse(
    ApplicationUser User,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
