using MassTransit;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Infrastructure.Services;

public sealed class RabbitMqDomainEventPublisher(
    IPublishEndpoint publishEndpoint
) : IDomainEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(domainEvent, cancellationToken);
    }

    public async Task PublishManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.PublishBatch(domainEvents, cancellationToken);
    }
}
