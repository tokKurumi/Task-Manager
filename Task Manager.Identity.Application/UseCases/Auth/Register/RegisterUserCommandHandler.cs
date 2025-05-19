using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IAuthService authService
) : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IAuthService _authService = authService;

    public async ValueTask<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(RegisterUserMapper.ToAuthServiceModel(request), cancellationToken);

        return result
            .MapError(error => new OneOfError<AuthError, ValidationError>(error))
            .Map(RegisterUserMapper.ToUseCaseModel);
    }
}
