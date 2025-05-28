namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record CreateUserCommand(
    Guid Id,
    string DisplayName
);
