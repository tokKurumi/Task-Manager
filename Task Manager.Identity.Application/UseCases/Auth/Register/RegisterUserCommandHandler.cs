using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

// TODO:
//  1. transactions with UNIT OF WORK
//  2. publish domain event to local databases
//  3. setup background job to get the domain event from the database and publish it to RabbitMQ
//  Current implementation is just simplification of perfect world scenario
public sealed class RegisterUserCommandHandler(
    IAuthService authService,
    IDomainEventPublisher domainEventPublisher
) : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>
{
    private readonly IAuthService _authService = authService;
    private readonly IDomainEventPublisher _domainEventPublisher = domainEventPublisher;

    public async ValueTask<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(RegisterUserMapper.ToAuthServiceModel(request), cancellationToken);

        return result
            .Tap(async response => await _domainEventPublisher.PublishManyAsync(response.User.DomainEvents))
            .MapError(error => new OneOfError<AuthError, ValidationError>(error))
            .Map(RegisterUserMapper.ToUseCaseModel);
    }
}
