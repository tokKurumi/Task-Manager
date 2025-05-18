using Task_Manager.Common;
using Task_Manager.Identity.Application.Jwt;
using Task_Manager.Identity.Core.Abstractions;
using Task_Manager.Identity.Core.Contracts;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services;

public class AuthService(
    IApplicationUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordService passwordService,
    TimeProvider timeProvider
) : IAuthService
{
    private readonly TimeSpan _expiresIn = TimeSpan.FromMinutes(30); // TODO: make it configurable

    private readonly IApplicationUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<LoginUserResponse, AuthError>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var userFindResult = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);
        if (userFindResult is null)
        {
            return new UserNotFoundError(request.Email);
        }

        var user = userFindResult.Value!;
        var passwordValid = _passwordService.VerifyHashedPassword(user.PasswordHash, request.Password);
        if (!passwordValid)
        {
            return new InvalidPasswordError(request.Email);
        }

        var issuedAt = _timeProvider.GetUtcNow();
        var expiresIn = _expiresIn;
        var jwtToken = _jwtTokenGenerator.GenerateToken(user, issuedAt, expiresIn);

        return new LoginUserResponse(user.Id, jwtToken.AccessToken, jwtToken.RefreshToken, issuedAt, expiresIn);
    }

    public async Task<Result<RegisterUserResponse, AuthError>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        var userFindResult = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);
        if (userFindResult is not null)
        {
            return new UserAlreadyExistError(request.Email);
        }

        var passwordHash = _passwordService.HashPassword(request.Password);
        var userCreateResult = ApplicationUser.TryCreate(request.Email, request.DisplayName, passwordHash, _timeProvider);
        if (userCreateResult.IsFailure)
        {
            return new CreationUserError(userCreateResult.Error!);
        }

        var user = userCreateResult.Value!;
        var userCreateRepositoryResult = await _userRepository.CreateUserAsync(user, request.Password, cancellationToken);
        if (userCreateRepositoryResult.IsFailure)
        {
            return new RepositoryCreateUserError(userCreateRepositoryResult.Error!);
        }

        var issuedAt = _timeProvider.GetUtcNow();
        var expiresIn = _expiresIn;
        var jwtToken = _jwtTokenGenerator.GenerateToken(user, issuedAt, expiresIn);

        return new RegisterUserResponse(user.Id, jwtToken.AccessToken, jwtToken.RefreshToken, issuedAt, expiresIn);
    }
}

public sealed record UserAlreadyExistError(string Email) : AuthError("UserAlreadyExists");

public sealed record CreationUserError(ApplicationUserError InnerError) : AuthError($"CreationUser.{InnerError.Code}");

public sealed record RepositoryCreateUserError(ApplicationUserRepositoryError InnerError) : AuthError($"RepositoryCreateUser.{InnerError.Code}");

public sealed record UserNotFoundError(string Email) : AuthError("UserNotFound");

public sealed record InvalidPasswordError(string Email) : AuthError("InvalidPassword");
