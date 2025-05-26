using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Abstractions;

public interface IUserRepository : IGenericRepository<User, UserRepositoryError>
{
    Task<Result<IReadOnlyList<User>, UsersNotFoundError>> GetUsersByIds(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
}

public abstract record UserRepositoryError : IError;

public sealed record UsersNotFoundError(IReadOnlyList<Guid> UserIds) : UserRepositoryError;
