using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Abstractions;

// TODO: add for identity micro-service too
public interface IGenericRepository<TEntity, TError>
    where TEntity : IAggregateRoot
    where TError : IError
{
    Task<Result<TEntity, TError>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<TEntity?, TError>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Page<TEntity>, TError>> GetPageAsync(IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TEntity, TError>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<TError>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
