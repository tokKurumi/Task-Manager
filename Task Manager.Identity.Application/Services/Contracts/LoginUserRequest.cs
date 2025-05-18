namespace Task_Manager.Identity.Application.Services.Contracts;

public sealed record LoginUserRequest(
    string Email,
    string Password
);