namespace Task_Manager.Identity.Core.Entities;

public interface IDomainModel
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
