namespace Task_Manager.TaskManagement.Core.Entities;

public interface IDomainModel
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}

public interface IDomainModel<in TData, out TDomainEntity> : IDomainModel
    where TDomainEntity : IDomainModel
{
    static abstract TDomainEntity ConvertFromData(TData input);
}
