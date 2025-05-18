using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task_Manager.Identity.Core.Abstractions;
using Task_Manager.Identity.Infrastructure.Data;
using Task_Manager.Identity.Infrastructure.Entities;
using Task_Manager.Identity.Infrastructure.Repositories;

namespace Task_Manager.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        return builder
            .AddRepositories()
            .AddIdentityDbContext();
    }

    private static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

        return builder;
    }

    private static IHostApplicationBuilder AddIdentityDbContext(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ApplicationIdentityDbContext>(Integrations.Identity.PostgreSQLDatabase);

        builder.Services.AddDataProtection();

        builder.Services.AddIdentityCore<UserEntity>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<UserEntity>>("Default");

        builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

        return builder;
    }
}
