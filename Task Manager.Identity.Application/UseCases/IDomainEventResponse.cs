using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.UseCases;

public interface IDomainEventResponse
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
