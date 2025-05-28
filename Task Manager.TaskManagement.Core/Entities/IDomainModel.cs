namespace Task_Manager.TaskManagement.Core.Entities;

public interface IDomainModel<in TData, out TDomainEntity>
{
    static abstract TDomainEntity ConvertFromData(TData input);
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
