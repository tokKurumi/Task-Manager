var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Task_Manager_Task_TaskAPI>("task-manager-task-taskapi");

builder.AddProject<Projects.Task_Manager_Task_Client>("task-manager-task-client");

builder.AddProject<Projects.Task_Manager_Identity_IdentityAPI>("task-manager-identity-identityapi");

builder.Build().Run();
