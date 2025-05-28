using Mediator;
using Task_Manager.Common;
using Task_Manager.Contracts.Identity.IntegrationEvents;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService,
    IIntegrationEventPublisher domainEventPublisher
) : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;
    private readonly IIntegrationEventPublisher _domainEventPublisher = domainEventPublisher;

    // TODO: use case logic and event publishing MUST be under one transaction in unit of work
    // So, in case when event publishing fails, the whole transaction will be rolled back
    // See: transactional outbox pattern for event publishing https://microservices.io/patterns/data/transactional-outbox.html
    public async ValueTask<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteWithStrategyAsync(async cancellationToken =>
            {
                return await _authService.RegisterAsync(RegisterUserMapper.ToAuthServiceModel(request), cancellationToken)
                    .Map(RegisterUserMapper.ToUseCaseModel)
                    .MapError(error => new OneOfError<AuthError, ValidationError>(error));
            }, cancellationToken)
            .TapAsync(response =>
            {
                var user = response.User;
                var integrationEvent = new UserCreatedIntegrationEvent(user.Id, user.Email, user.DisplayName, user.CreatedAt);
                return _domainEventPublisher.PublishAsync(integrationEvent, cancellationToken);
            });
    }
}
