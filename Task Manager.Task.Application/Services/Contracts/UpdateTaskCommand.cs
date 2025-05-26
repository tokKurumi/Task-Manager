namespace Task_Manager.Task.Application.Services.Contracts;

public record UpdateTaskCommand(
    Guid UserPerformerId,
    Guid TaskId,
    string Title,
    string Description,
    string Notes,
    DateTime? ApproximateCompletedAt
);
