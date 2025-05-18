using Task_Manager.Common;

namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record PasswordPolicyError(IReadOnlyCollection<(string Code, string Description)> Errors) : Error("PasswordPolicyError");
