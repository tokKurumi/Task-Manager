using Task_Manager.AppHost.Extensions;
using Task_Manager.Integrations;

var builder = DistributedApplication.CreateBuilder(args);

var jwtParameters = builder.AddJwtParameters();

var rabbitMq = builder
    .AddRabbitMQ(MessageBroker.RabbitMQ)
    .WithDataVolume(MessageBroker.RabbitMQVolume)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

#region identity
var identityDatabase = builder
    .AddPostgres(IdentityProject.PostgreSQLResource)
    .WithDataVolume(IdentityProject.PostgreSQLVolume)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .AddDatabase(IdentityProject.PostgreSQLDatabase);

var identityMigrator = builder
    .AddProject<Projects.Task_Manager_Identity_Migrator>(IdentityProject.Migrator)
    .WithJwtParameters(jwtParameters)
    .WithReference(identityDatabase)
    .WithReference(rabbitMq)
    .WaitFor(identityDatabase);

var identity = builder
    .AddProject<Projects.Task_Manager_Identity_IdentityAPI>(IdentityProject.API)
    .WithJwtParameters(jwtParameters)
    .WithReference(identityDatabase)
    .WithReference(rabbitMq)
    .WaitFor(identityDatabase)
    .WaitFor(rabbitMq)
    .WaitForCompletion(identityMigrator);
#endregion

builder
    .AddProject<Projects.Task_Manager_Task_TaskAPI>(TaskProject.API)
    .WithJwtParameters(jwtParameters);

await builder.Build().RunAsync();
