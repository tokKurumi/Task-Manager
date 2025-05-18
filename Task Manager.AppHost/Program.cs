using Task_Manager.Integrations;

var builder = DistributedApplication.CreateBuilder(args);

#region identity
var identityDatabase = builder
    .AddPostgres(Identity.PostgreSQLResource)
    .WithDataVolume(Identity.PostgreSQLVolume)
    .WithPgAdmin()
    .AddDatabase(Identity.PostgreSQLDatabase);

var identityMigrator = builder
    .AddProject<Projects.Task_Manager_Identity_Migrator>("task-manager-identity-migrator")
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase);

var identity = builder
    .AddProject<Projects.Task_Manager_Identity_IdentityAPI>("task-manager-identity-identityapi")
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase)
    .WaitForCompletion(identityMigrator);
#endregion

builder.AddProject<Projects.Task_Manager_Task_Client>("task-manager-task-client");

builder.AddProject<Projects.Task_Manager_Task_TaskAPI>("task-manager-task-taskapi");

await builder.Build().RunAsync();
