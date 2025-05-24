using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService,
    IDomainEventPublisher domainEventPublisher
) : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;
    private readonly IDomainEventPublisher _domainEventPublisher = domainEventPublisher;

    public async ValueTask<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ExecuteWithStrategyAsync(async cancellationToken =>
        {
            var result = await _authService.RegisterAsync(RegisterUserMapper.ToAuthServiceModel(request), cancellationToken);

            return result
                .Map(RegisterUserMapper.ToUseCaseModel)
                .MapError(error => new OneOfError<AuthError, ValidationError>(error));
        }, cancellationToken);

        return result
            .Tap(async response => await _domainEventPublisher.PublishManyAsync(response.DomainEvents, cancellationToken));
    }
}
