﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Entities;
using IdentityFrameworkError = Microsoft.AspNetCore.Identity.IdentityError;

namespace Task_Manager.Identity.Infrastructure.Repositories;

public class ApplicationUserRepository(
    UserManager<UserEntity> userManager
) : IApplicationUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;

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

    public async Task<bool> IsUniqueEmail(string email, CancellationToken cancellationToken = default)
    {
        return !await _userManager.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Result<ApplicationUser?, ApplicationUserRepositoryError>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser is null)
        {
            return Result<ApplicationUser?, ApplicationUserRepositoryError>.Success(null);
        }

        var domainModelMapResult = ApplicationUser.TryConvertFromData(identityUser);
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

        var domainModelMapResult = ApplicationUser.TryConvertFromData(identityUser);
        if (domainModelMapResult.IsFailure)
        {
            return new InnerDomainError(domainModelMapResult.Error!);
        }

        return domainModelMapResult.Value;
    }

    public async Task<Result<Page<ApplicationUser>, ApplicationUserRepositoryError>> GetPageAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var identityUsers = await _userManager.Users
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _userManager.Users.CountAsync(cancellationToken);

        var domainModelMapResults = identityUsers.ConvertAll(ApplicationUser.TryConvertFromData).ToList();
        if (domainModelMapResults.Any(result => result.IsFailure))
        {
            return new InnerDomainErrors([.. domainModelMapResults.Select(result => result.Error!)]);
        }

        var domainModels = domainModelMapResults.Select(result => result.Value!).ToList();

        return new Page<ApplicationUser>(domainModels, pagination.Page, pagination.PageSize, totalCount);
    }
}

public sealed record IdentityError(IReadOnlyCollection<IdentityFrameworkError> InnerErrors) : ApplicationUserRepositoryError;

public sealed record InnerDomainError(CreateApplicationUserError InnerError) : ApplicationUserRepositoryError;

public sealed record InnerDomainErrors(IReadOnlyCollection<CreateApplicationUserError> InnerErrors) : ApplicationUserRepositoryError;