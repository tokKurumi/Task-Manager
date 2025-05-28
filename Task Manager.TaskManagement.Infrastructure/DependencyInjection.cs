using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Infrastructure.Data;
using Task_Manager.TaskManagement.Infrastructure.Repositories;
using Task_Manager.TaskManagement.Infrastructure.Services;

namespace Task_Manager.TaskManagement.Infrastructure;

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
        return builder;
    }

    private static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return builder;
    }

    private static IHostApplicationBuilder AddIdentityDbContext(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<TaskManagementDbContext>(Integrations.TaskManagmentProject.PostgreSQLDatabase);

        return builder;
    }

    private static IHostApplicationBuilder AddMessaging(this IHostApplicationBuilder builder)
    {
        builder.AddMassTransitRabbitMq(Integrations.MessageBroker.RabbitMQ);

        return builder;
    }
}
