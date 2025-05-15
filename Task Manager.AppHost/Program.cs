var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Task_Manager_TaskAPI>("task manager-taskapi");

builder.AddProject<Projects.Task_Manager_Client>("task manager-client");

builder.Build().Run();
