using Microsoft.AspNetCore.Identity;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Entities;

namespace Task_Manager.Identity.Infrastructure.Services;

public class PasswordService(
    UserManager<UserEntity> userManager
) : IPasswordService
{
    private readonly UserManager<UserEntity> _userManager = userManager;

    public string HashPassword(ApplicationUser user, string password)
    {
        var userEntity = new UserEntity(user);

        return _userManager.PasswordHasher.HashPassword(userEntity, password);
    }

    public async Task<bool> VerifyHashedPassword(ApplicationUser user, string providedPassword)
    {
        var userEntity = await _userManager.FindByIdAsync(user.Id.ToString());
        if (userEntity is null)
        {
            return false;
        }

        return await _userManager.CheckPasswordAsync(userEntity, providedPassword);
    }
}
