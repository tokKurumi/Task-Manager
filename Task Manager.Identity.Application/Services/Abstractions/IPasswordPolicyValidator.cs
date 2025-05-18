using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Contracts;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IPasswordPolicyValidator
{
    Task<Result<PasswordPolicyError>> ValidateAsync(string password, CancellationToken cancellationToken = default);
}
