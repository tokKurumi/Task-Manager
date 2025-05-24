namespace Task_Manager.Task.Core.Entities;

public interface IDomainModel
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
