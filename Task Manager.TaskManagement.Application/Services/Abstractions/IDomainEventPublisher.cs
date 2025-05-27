using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task PublishManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
