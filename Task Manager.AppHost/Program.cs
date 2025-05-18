using Task_Manager.Integrations;

var builder = DistributedApplication.CreateBuilder(args);

#region identity
var identityDatabase = builder
    .AddPostgres(IdentityProject.PostgreSQLResource)
    .WithDataVolume(IdentityProject.PostgreSQLVolume)
    .WithPgAdmin()
    .AddDatabase(IdentityProject.PostgreSQLDatabase);

var identityMigrator = builder
    .AddProject<Projects.Task_Manager_Identity_Migrator>(IdentityProject.Migrator)
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase);

var identity = builder
    .AddProject<Projects.Task_Manager_Identity_IdentityAPI>(IdentityProject.API)
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase)
    .WaitForCompletion(identityMigrator);
#endregion

builder.AddProject<Projects.Task_Manager_Task_Client>(TaskProject.Client);

builder.AddProject<Projects.Task_Manager_Task_TaskAPI>(TaskProject.API);

await builder.Build().RunAsync();
