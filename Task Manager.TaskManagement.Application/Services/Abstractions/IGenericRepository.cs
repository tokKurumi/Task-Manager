using Task_Manager.Common;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public abstract record RepositoryError : IError;

public sealed record NotFoundError(Guid Id) : RepositoryError;

public interface IGenericRepository<TEntity, TError>
    where TEntity : IAggregateRoot
    where TError : RepositoryError
{
    Task<Result<TEntity, TError>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TEntity?, TError>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Page<TEntity>, TError>> GetPageAsync(IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TError>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
