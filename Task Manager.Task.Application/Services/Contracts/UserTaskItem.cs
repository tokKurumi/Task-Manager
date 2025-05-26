using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Contracts;

public record UserTaskItem(User User, TaskItem TaskItem);

