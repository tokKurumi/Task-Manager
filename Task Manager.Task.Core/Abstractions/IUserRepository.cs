using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Core.Abstractions;

public interface IUserRepository
{
    Task<Result<User, UserRepositoryError>> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Page<User>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public abstract record UserRepositoryError : IError;
