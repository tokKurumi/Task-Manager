namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
);
