namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

public record LoginUserRequest(
    string Email,
    string Password
);
