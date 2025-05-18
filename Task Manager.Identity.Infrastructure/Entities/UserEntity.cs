using Microsoft.AspNetCore.Identity;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Infrastructure.Entities;

public class UserEntity : IdentityUser<Guid>, IUserData
{
    public override Guid Id => base.Id;
    public override string Email => base.Email ?? string.Empty;
    public string DisplayName { get => base.UserName ?? string.Empty; set => base.UserName = value; }
    public DateTimeOffset CreatedAt { get; set; }

    public UserEntity() { }

    public UserEntity(ApplicationUser domainUser)
    {
        Id = domainUser.Id;
        Email = domainUser.Email;
        DisplayName = domainUser.DisplayName;
        CreatedAt = domainUser.CreatedAt;
    }
}
