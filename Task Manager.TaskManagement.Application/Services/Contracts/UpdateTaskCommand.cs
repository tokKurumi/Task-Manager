namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record UpdateTaskCommand(
    Guid UserPerformerId,
    Guid TaskId,
    string Title,
    string Description,
    string Notes,
    DateTime? ApproximateCompletedAt
);
