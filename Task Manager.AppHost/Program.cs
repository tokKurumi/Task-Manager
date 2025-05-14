var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Task_Manager_WebAPI>("task manager-webapi");

builder.AddProject<Projects.Task_Manager_Client>("task manager-client");

builder.Build().Run();
