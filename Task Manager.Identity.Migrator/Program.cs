using Task_Manager.Identity.Infrastructure.Extensions;
using Task_Manager.Identity.Migrator;
using Task_Manager.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddIdentityDbContext();

builder.Services.AddHostedService<Worker>();

builder.Services
    .AddOpenTelemetry()
    .WithTracing(config =>
    {
        config.AddSource(Worker.ActivitySourceName);
    });

var host = builder.Build();

host.Run();
