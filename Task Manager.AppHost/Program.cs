using Task_Manager.Integrations;

var builder = DistributedApplication.CreateBuilder(args);

var identityDatabase = builder
    .AddPostgres(Identity.PostgreSQLResource)
    .WithDataVolume(Identity.PostgreSQLVolume)
    .WithPgAdmin()
    .AddDatabase(Identity.PostgreSQLDatabase);

var identity = builder
    .AddProject<Projects.Task_Manager_Task_TaskAPI>("task-manager-task-taskapi")
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase);

builder.AddProject<Projects.Task_Manager_Task_Client>("task-manager-task-client");

builder.AddProject<Projects.Task_Manager_Identity_IdentityAPI>("task-manager-identity-identityapi");

builder.Build().Run();
