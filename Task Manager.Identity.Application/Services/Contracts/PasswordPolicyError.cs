using Task_Manager.Common;

namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record PasswordPolicyError(IReadOnlyCollection<PasswordPolicyErrorItem> Errors) : Error("PasswordPolicyError");

public sealed record PasswordPolicyErrorItem(string Code, string Description);
