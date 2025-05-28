using Task_Manager.Contracts;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    Task PublishManyAsync(IEnumerable<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default);
}
