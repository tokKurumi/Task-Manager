using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Identity.Infrastructure.Configuration;
using Task_Manager.Identity.Infrastructure.Entities;

namespace Task_Manager.Identity.Infrastructure.Data;

public class ApplicationIdentityDbContext(
    DbContextOptions<ApplicationIdentityDbContext> options
) : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new UserEntityConfiguration());
    }
}
