namespace Task_Manager.Identity.Core.Contracts;

public sealed record LoginUserRequest(
    string Email,
    string Password
);