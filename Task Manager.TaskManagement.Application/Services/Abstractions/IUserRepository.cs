using Task_Manager.Common;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public interface IUserRepository : IGenericRepository<User, UserRepositoryError>
{
    Task<Result<IReadOnlyList<User>, UsersNotFoundError>> GetUsersByIds(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
}

public abstract record UserRepositoryError : RepositoryError;

public sealed record UsersNotFoundError(IReadOnlyList<Guid> UserIds) : UserRepositoryError;
