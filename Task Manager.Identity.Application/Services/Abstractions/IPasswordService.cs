using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IPasswordService
{
    string HashPassword(ApplicationUser user, string password);
    Task<bool> VerifyHashedPassword(ApplicationUser user, string providedPassword);
}
