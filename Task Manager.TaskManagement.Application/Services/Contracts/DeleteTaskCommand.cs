namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record DeleteTaskCommand(
    Guid UserPerformerId,
    Guid TaskId
);
