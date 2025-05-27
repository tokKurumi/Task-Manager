namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record AddCommentCommand(Guid UserPerformerId, Guid TaskId, string Message);

