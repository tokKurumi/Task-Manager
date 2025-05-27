using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

public sealed class LoginUserRequestHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService
) : IRequestHandler<LoginUserRequest, Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;

    // TODO: each use case logic should be wrapped in a transaction to ensure consistency
    // this needs to be implemented using pipeline behavior
    public async ValueTask<Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteWithStrategyAsync(async cancelationToken =>
        {
            return await _authService.LoginAsync(LoginUserMapper.ToAuthServiceModel(request), cancellationToken)
                .Map(LoginUserMapper.ToUseCaseModel)
                .MapError(error => new OneOfError<AuthError, ValidationError>(error));
        }, cancellationToken);
    }
}
