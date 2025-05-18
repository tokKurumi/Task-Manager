using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Core.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password
) : IRequest<Result<LoginUserResponse, AuthError>>;

public sealed record LoginUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);