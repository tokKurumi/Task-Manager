using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.Identity.Core.Abstractions;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Entities;
using Task_Manager.Identity.Infrastructure.Mappers;
using IdentityFrameworkError = Microsoft.AspNetCore.Identity.IdentityError;

namespace Task_Manager.Identity.Infrastructure.Repositories;

public class ApplicationUserRepository(
    UserManager<UserEntity> userManager,
    ApplicationUserMapper userMapper
) : IApplicationUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<Result<ApplicationUser, ApplicationUserRepositoryError>> CreateUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        var identityUser = ApplicationUserMapper.MapToInfrastructureModel(user);
        var userCreateResult = await _userManager.CreateAsync(identityUser, password);
        if (!userCreateResult.Succeeded)
        {
            return Result<ApplicationUser, ApplicationUserRepositoryError>.Failure(new IdentityError([.. userCreateResult.Errors]));
        }

        return Result<ApplicationUser, ApplicationUserRepositoryError>.Success(user);
    }

    public async Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser is null)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(null);
        }

        var domainModelMapResult = userMapper.TryMapToDomainModel(identityUser);
        if (domainModelMapResult.IsFailure)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Failure(new InnerDomainError(domainModelMapResult.Error!));
        }

        return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(domainModelMapResult.Value);
    }

    public async Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString()); // WTF Microsoft? Why not Guid for generic IdentityUser<Guid>?
        if (identityUser is null)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(null);
        }

        var domainModelMapResult = userMapper.TryMapToDomainModel(identityUser);
        if (domainModelMapResult.IsFailure)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Failure(new InnerDomainError(domainModelMapResult.Error!));
        }

        return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(domainModelMapResult.Value);
    }

    public async Task<Result<Page<ApplicationUser>, ApplicationUserRepositoryError>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var identityUsers = await _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _userManager.Users.CountAsync(cancellationToken);

        var domainModelMapResults = identityUsers.ConvertAll(userMapper.TryMapToDomainModel).ToList();
        if (domainModelMapResults.Any(result => result.IsFailure))
        {
            return Result<Page<ApplicationUser>, ApplicationUserRepositoryError>.Failure(
                new InnerDomainErrors([.. domainModelMapResults.Select(result => result.Error!)])
            );
        }

        var domainModels = domainModelMapResults.Select(result => result.Value!).ToList();

        return Result<Page<ApplicationUser>, ApplicationUserRepositoryError>.Success(
            new Page<ApplicationUser>(domainModels, pageNumber, pageSize, totalCount)
        );
    }
}

public sealed record IdentityError(IReadOnlyCollection<IdentityFrameworkError> InnerErrors) : ApplicationUserRepositoryError("IdentityError");

public sealed record InnerDomainError(ApplicationUserError InnerError) : ApplicationUserRepositoryError($"Domain.{InnerError.Code}");

public sealed record InnerDomainErrors(IReadOnlyCollection<ApplicationUserError> InnerErrors) : ApplicationUserRepositoryError($"Domain.MultipleErrors");