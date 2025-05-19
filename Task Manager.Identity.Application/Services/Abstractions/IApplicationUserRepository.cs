using Task_Manager.Common;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IApplicationUserRepository
{
    Task<Result<ApplicationUser, ApplicationUserRepositoryError>> CreateUserAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken = default);
    Task<bool> IsUniqueEmail(string email, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Page<ApplicationUser>, ApplicationUserRepositoryError>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public abstract record ApplicationUserRepositoryError : IError;
