using Microsoft.AspNetCore.Identity;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Infrastructure.Entities;

public class UserEntity : IdentityUser<Guid>, IUserData
{
    public override Guid Id => base.Id;

    string IUserData.Email => Email ?? string.Empty;
    public override string? Email
    {
        get => base.Email;
        set
        {
            base.Email = value;
            base.UserName = value;
        }
    }

    public string DisplayName { get; set; } = string.Empty;

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
