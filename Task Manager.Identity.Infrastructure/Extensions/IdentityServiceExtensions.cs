using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task_Manager.Identity.Core.Abstractions;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Data;
using Task_Manager.Identity.Infrastructure.Entities;
using Task_Manager.Identity.Infrastructure.Repositories;

namespace Task_Manager.Identity.Infrastructure.Extensions;

public static class IdentityServiceExtensions
{
    public static IHostApplicationBuilder AddIdentityDbContext(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        builder.AddNpgsqlDbContext<ApplicationIdentityDbContext>("identity-postgres"); // TODO: remove this hard-code

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<UserEntity>>("Default");

        builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

        builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

        return builder;
    }
}
