using Task_Manager.Common;
using Task_Manager.Identity.Core.Contracts;

namespace Task_Manager.Identity.Core.Abstractions;

public interface IAuthService
{
    Task<Result<RegisterUserResponse, AuthError>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default);
    Task<Result<LoginUserResponse, AuthError>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
}

public abstract record AuthError(string Code) : Error(Code);
