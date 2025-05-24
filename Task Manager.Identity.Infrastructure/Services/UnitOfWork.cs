using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Infrastructure.Data;

namespace Task_Manager.Identity.Infrastructure.Services;

public class UnitOfWork(
    ApplicationIdentityDbContext dbContext
) : IUnitOfWork
{
    private readonly ApplicationIdentityDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    public async Task<T> ExecuteWithStrategyAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async ct =>
        {
            await BeginTransactionAsync(ct);

            try
            {
                return await operation(ct);
            }
            catch
            {
                await RollbackTransactionAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction was started.");
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync();
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
