using MassTransit;
using Task_Manager.Contracts;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Infrastructure.Services;

public sealed class RabbitMqIntegrationEventPublisher(
    IPublishEndpoint publishEndpoint
) : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
    }

    public async Task PublishManyAsync(IEnumerable<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.PublishBatch(integrationEvents, cancellationToken);
    }
}
