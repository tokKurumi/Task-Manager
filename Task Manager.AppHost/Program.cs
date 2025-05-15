var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Task_Manager_Task_TaskAPI>("task-manager-task-taskapi");

builder.AddProject<Projects.Task_Manager_Task_Client>("task-manager-task-client");

builder.Build().Run();
