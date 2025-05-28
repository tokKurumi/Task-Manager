using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Core.Entities;
using Task_Manager.TaskManagement.Infrastructure.Data;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Repositories;

public class UserRepository(
    TaskManagementDbContext context,
    Func<Guid, UserRepositoryError> notFoundErrorFactory
) : GenericRepository<User, UserEntity, UserRepositoryError>(context, notFoundErrorFactory), IUserRepository
{
    private readonly TaskManagementDbContext _context = context;

    public async Task<Result<IReadOnlyList<User>, UsersNotFoundError>> GetUsersByIds(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var userEntities = await _context.Users.AsNoTracking()
            .Where(user => userIds.Contains(user.Id))
            .ToListAsync(cancellationToken);

        if (userEntities.Count == 0)
        {
            return new UsersNotFoundError([.. userIds]);
        }

        return userEntities.Select(User.ConvertFromData).ToList();
    }

}
