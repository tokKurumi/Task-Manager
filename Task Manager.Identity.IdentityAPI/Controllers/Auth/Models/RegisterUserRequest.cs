namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

public record RegisterUserRequest(
    string Email,
    string DisplayName,
    string Password
);
