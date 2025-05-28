using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Entities;

public interface IInfrastructureEntity<in TDomainEntity, out TInfrastructureEntity>
    where TDomainEntity : IDomainModel
{
    static abstract TInfrastructureEntity Create(TDomainEntity input);
    Guid Id { get; }
}
