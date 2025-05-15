using Task_Manager.Common;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Core.Abstractions;

public interface IApplicationUserRepository
{
    Task<Result<ApplicationUser, ApplicationUserRepositoryError>> CreateUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Page<ApplicationUser>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public abstract record ApplicationUserRepositoryError(string Code) : Error(Code);