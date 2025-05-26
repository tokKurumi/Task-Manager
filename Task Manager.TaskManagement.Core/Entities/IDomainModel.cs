namespace Task_Manager.TaskManagement.Core.Entities;

public interface IDomainModel
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
