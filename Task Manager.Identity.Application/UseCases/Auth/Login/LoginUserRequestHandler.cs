using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Core.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

public sealed class LoginUserRequestHandler(
    IAuthService authService
) : IRequestHandler<LoginUserRequest, Result<LoginUserResponse, AuthError>>
{
    private readonly IAuthService _authService = authService;

    public async ValueTask<Result<LoginUserResponse, AuthError>> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(LoginUserMapper.ToCore(request), cancellationToken);

        return result.Map(LoginUserMapper.ToApplication);
    }
}
