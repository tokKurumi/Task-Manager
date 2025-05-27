namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<T> ExecuteWithStrategyAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
