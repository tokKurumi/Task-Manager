using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record UserTaskItem(User User, TaskItem TaskItem);

