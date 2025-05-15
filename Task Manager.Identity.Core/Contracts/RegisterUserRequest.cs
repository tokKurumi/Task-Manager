namespace Task_Manager.Identity.Core.Contracts;

public sealed record RegisterUserRequest(
    string Email,
    string DisplayName,
    string Password
);
