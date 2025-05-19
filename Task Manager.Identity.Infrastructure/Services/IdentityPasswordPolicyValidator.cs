using Microsoft.AspNetCore.Identity;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Application.Services.Contracts;
using Task_Manager.Identity.Infrastructure.Entities;

namespace Task_Manager.Identity.Infrastructure.Services;

public class IdentityPasswordPolicyValidator(
    IPasswordValidator<UserEntity> passwordValidator,
    UserManager<UserEntity> userManager
) : IPasswordPolicyValidator
{
    private readonly IPasswordValidator<UserEntity> _passwordValidator = passwordValidator;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<Result<PasswordPolicyError>> ValidateAsync(string password, CancellationToken cancellationToken = default)
    {
        var dummy = new UserEntity() { UserName = "Dummy" };

        var result = await _passwordValidator.ValidateAsync(_userManager, dummy, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .ToList()
                .ConvertAll(error => new PasswordPolicyErrorItem(error.Code, error.Description));

            return new PasswordPolicyError(errors);
        }

        return Result<PasswordPolicyError>.Success();
    }
}
