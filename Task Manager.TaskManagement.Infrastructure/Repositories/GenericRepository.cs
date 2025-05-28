using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Core.Entities;
using Task_Manager.TaskManagement.Infrastructure.Data;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Repositories;

public class GenericRepository<TDomainEntity, TInfraEntity, TError>(
    TaskManagementDbContext context,
    Func<Guid, TError> notFoundErrorFactory
) : IGenericRepository<TDomainEntity, TError>
    where TDomainEntity : class, IAggregateRoot, IDomainModel, IDomainModel<TInfraEntity, TDomainEntity>
    where TInfraEntity : class, IInfrastructureEntity<TDomainEntity, TInfraEntity>, IDomainModel
    where TError : RepositoryError
{
    private readonly DbSet<TInfraEntity> _dbSet = context.Set<TInfraEntity>();
    private readonly Func<Guid, TError> _notFoundErrorFactory = notFoundErrorFactory;

    public async Task<Result<TDomainEntity, TError>> CreateAsync(TDomainEntity entity, CancellationToken cancellationToken = default)
    {
        var infraEntity = TInfraEntity.Create(entity);
        await _dbSet.AddAsync(infraEntity, cancellationToken);
        return Result<TDomainEntity, TError>.Success(entity);
    }

    public async Task<Result<TError>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var infraEntity = await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        if (infraEntity is null)
        {
            return _notFoundErrorFactory(id);
        }

        _dbSet.Remove(infraEntity);
        return Result<TError>.Success();
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().AnyAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<Result<TDomainEntity?, TError>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var infraEntity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        if (infraEntity is null)
        {
            return Result<TDomainEntity?, TError>.Success(null);
        }

        return TDomainEntity.ConvertFromData(infraEntity);
    }

    public async Task<Result<Page<TDomainEntity>, TError>> GetPageAsync(IPagination pagination, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var domainEntities = items.Select(TDomainEntity.ConvertFromData).ToList();
        return new Page<TDomainEntity>(domainEntities, pagination, totalCount);
    }
}
