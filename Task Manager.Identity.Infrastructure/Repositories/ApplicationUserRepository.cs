using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Entities;
using IdentityFrameworkError = Microsoft.AspNetCore.Identity.IdentityError;

namespace Task_Manager.Identity.Infrastructure.Repositories;

public class ApplicationUserRepository(
    UserManager<UserEntity> userManager,
    TimeProvider timeProvider
) : IApplicationUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<ApplicationUser, ApplicationUserRepositoryError>> CreateUserAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken = default)
    {
        var identityUser = new UserEntity(user)
        {
            PasswordHash = passwordHash
        };

        var userCreateResult = await _userManager.CreateAsync(identityUser);
        if (!userCreateResult.Succeeded)
        {
            return new IdentityError([.. userCreateResult.Errors]);
        }

        return user;
    }

    public async Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser is null)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(null);
        }

        var domainModelMapResult = ApplicationUser.TryCreate(identityUser, _timeProvider);
        if (domainModelMapResult.IsFailure)
        {
            return new InnerDomainError(domainModelMapResult.Error!);
        }

        return domainModelMapResult.Value;
    }

    public async Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString()); // WTF Microsoft? Why not Guid for generic IdentityUser<Guid>?
        if (identityUser is null)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(null);
        }

        var domainModelMapResult = ApplicationUser.TryCreate(identityUser, _timeProvider);
        if (domainModelMapResult.IsFailure)
        {
            return new InnerDomainError(domainModelMapResult.Error!);
        }

        return domainModelMapResult.Value;
    }

    public async Task<Result<Page<ApplicationUser>, ApplicationUserRepositoryError>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var identityUsers = await _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _userManager.Users.CountAsync(cancellationToken);

        var domainModelMapResults = identityUsers.ConvertAll(identityUser => ApplicationUser.TryCreate(identityUser, _timeProvider)).ToList();
        if (domainModelMapResults.Any(result => result.IsFailure))
        {
            return new InnerDomainErrors([.. domainModelMapResults.Select(result => result.Error!)]);
        }

        var domainModels = domainModelMapResults.Select(result => result.Value!).ToList();

        return new Page<ApplicationUser>(domainModels, pageNumber, pageSize, totalCount);
    }
}

public sealed record IdentityError(IReadOnlyCollection<IdentityFrameworkError> InnerErrors) : ApplicationUserRepositoryError("IdentityError");

public sealed record InnerDomainError(ApplicationUserError InnerError) : ApplicationUserRepositoryError($"Domain.{InnerError.Code}");

public sealed record InnerDomainErrors(IReadOnlyCollection<ApplicationUserError> InnerErrors) : ApplicationUserRepositoryError($"Domain.MultipleErrors");