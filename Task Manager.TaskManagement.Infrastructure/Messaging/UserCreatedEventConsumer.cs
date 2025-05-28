using MassTransit;
using Mediator;
using Task_Manager.Contracts.Identity.IntegrationEvents;
using Task_Manager.TaskManagement.Application.IntegrationEventHandlers.UserCreateEvent;

namespace Task_Manager.TaskManagement.Infrastructure.Messaging;

public sealed class UserCreatedEventConsumer(
    ISender sender
) : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ISender _sender = sender;

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var command = new CreateUserCommand(
            context.Message.Id,
            context.Message.DisplayName
        );

        await _sender.Send(command, context.CancellationToken);
    }
}
