namespace Task_Manager.Identity.Core.Contracts;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
);
