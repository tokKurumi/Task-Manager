using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Application.Services.Contracts;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services;

public abstract record AuthError : IError;

public interface IAuthService
{
    Task<Result<RegisterUserResponse, AuthError>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default);
    Task<Result<LoginUserResponse, AuthError>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
}

public class AuthService(
    IApplicationUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordPolicyValidator passwordPolicyValidator,
    IPasswordService passwordService,
    TimeProvider timeProvider
) : IAuthService
{
    private readonly IApplicationUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordPolicyValidator _passwordPolicyValidator = passwordPolicyValidator;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<LoginUserResponse, AuthError>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var userFindResult = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);
        if (userFindResult.IsFailure)
        {
            return new RepositoryCreateUserError(userFindResult.Error!);
        }

        var user = userFindResult.Value;
        if (user is null)
        {
            return new UserNotFoundError(request.Email);
        }

        var passwordValid = await _passwordService.VerifyHashedPassword(user, request.Password);
        if (!passwordValid)
        {
            return new InvalidPasswordError(request.Email);
        }

        var jwtToken = _jwtTokenGenerator.GenerateToken(user);

        return new LoginUserResponse(user.Id, jwtToken.AccessToken, jwtToken.RefreshToken, jwtToken.IssuedAt, jwtToken.ExpiresIn);
    }

    public async Task<Result<RegisterUserResponse, AuthError>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        var uniqueEmail = await _userRepository.IsUniqueEmail(request.Email, cancellationToken);
        if (!uniqueEmail)
        {
            return new UserAlreadyExistError(request.Email);
        }

        var passwordPolicyValidationResult = await _passwordPolicyValidator.ValidateAsync(request.Password, cancellationToken);
        if (passwordPolicyValidationResult.IsFailure)
        {
            return new PasswordPolicyValidationResult(passwordPolicyValidationResult.Error!);
        }

        var userCreateResult = ApplicationUser.TryCreate(request.Email, request.DisplayName, _timeProvider);
        if (userCreateResult.IsFailure)
        {
            return new CreationUserError(userCreateResult.Error!);
        }

        var createdUser = userCreateResult.Value!;
        var passwordHash = _passwordService.HashPassword(createdUser, request.Password);
        var userCreateRepositoryResult = await _userRepository.CreateUserAsync(createdUser, passwordHash, cancellationToken);
        if (userCreateRepositoryResult.IsFailure)
        {
            return new RepositoryCreateUserError(userCreateRepositoryResult.Error!);
        }

        var jwtToken = _jwtTokenGenerator.GenerateToken(createdUser);

        return new RegisterUserResponse(createdUser, jwtToken.AccessToken, jwtToken.RefreshToken, jwtToken.IssuedAt, jwtToken.ExpiresIn);
    }
}

public sealed record UserAlreadyExistError(string Email) : AuthError;

public sealed record CreationUserError(CreateApplicationUserError InnerError) : AuthError;

public sealed record RepositoryCreateUserError(ApplicationUserRepositoryError InnerError) : AuthError;

public sealed record PasswordPolicyValidationResult(PasswordPolicyError InnerError) : AuthError;

public sealed record UserNotFoundError(string Email) : AuthError;

public sealed record InvalidPasswordError(string Email) : AuthError;
