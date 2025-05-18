using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Core.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IAuthService authService
) : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, AuthError>>
{
    private readonly IAuthService _authService = authService;

    public async ValueTask<Result<RegisterUserResponse, AuthError>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(RegisterUserMapper.ToCore(request), cancellationToken);

        return result.Map(RegisterUserMapper.ToApplication);
    }
}