using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Infrastructure.Data;
using Task_Manager.Identity.Infrastructure.Entities;
using Task_Manager.Identity.Infrastructure.Repositories;
using Task_Manager.Identity.Infrastructure.Services;

namespace Task_Manager.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        return builder
            .AddServices()
            .AddRepositories()
            .AddIdentityDbContext()
            .AddMessaging();
    }

    public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPasswordPolicyValidator, IdentityPasswordPolicyValidator>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return builder;
    }

    private static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

        return builder;
    }

    private static IHostApplicationBuilder AddIdentityDbContext(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ApplicationIdentityDbContext>(Integrations.IdentityProject.PostgreSQLDatabase);

        builder.Services.AddDataProtection();

        builder.Services.AddIdentityCore<UserEntity>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireUppercase = false;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<UserEntity>>("Default");

        builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

        return builder;
    }

    private static IHostApplicationBuilder AddMessaging(this IHostApplicationBuilder builder)
    {
        builder.AddMassTransitRabbitMq(Integrations.MessageBroker.RabbitMQ);

        builder.Services.AddScoped<IDomainEventPublisher, RabbitMqDomainEventPublisher>();

        return builder;
    }
}
