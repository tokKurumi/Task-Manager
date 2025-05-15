using Task_Manager.Common;
using Task_Manager.Core.Entities;

namespace Task_Manager.Core.Abstractions;

public interface IUserRepository
{
    Task<Result<User, UserError>> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<User, UserError>> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Page<User>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
