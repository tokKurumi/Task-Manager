using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Core.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
) : ICommand<Result<RegisterUserResponse, AuthError>>;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);