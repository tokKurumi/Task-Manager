using Task_Manager.Common;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<Result<T, TError>> ExecuteWithStrategyAsync<T, TError>(
        Func<CancellationToken, Task<Result<T, TError>>> operation,
        CancellationToken cancellationToken = default
    ) where TError : IError;
    Task<Result<TError>> ExecuteWithStrategyAsync<TError>(
        Func<CancellationToken, Task<Result<TError>>> operation,
        CancellationToken cancellationToken = default
    ) where TError : IError;
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
