using Task_Manager.Common;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IPasswordPolicyValidator
{
    Task<Result<PasswordPolicyError>> ValidateAsync(string password, CancellationToken cancellationToken = default);
}

public sealed record PasswordPolicyError(IReadOnlyCollection<(string Code, string Description)> Errors) : Error("PasswordPolicyError");
