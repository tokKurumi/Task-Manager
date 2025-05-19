using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

public sealed class LoginUserRequestHandler(
    IAuthService authService
) : IRequestHandler<LoginUserRequest, Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IAuthService _authService = authService;

    public async ValueTask<Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(LoginUserMapper.ToAuthServiceModel(request), cancellationToken);

        return result
            .MapError(error => new OneOfError<AuthError, ValidationError>(error))
            .Map(LoginUserMapper.ToUseCaseModel);
    }
}
