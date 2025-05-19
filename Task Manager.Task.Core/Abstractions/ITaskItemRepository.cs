using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Core.Abstractions;

public interface ITaskItemRepository
{
    Task<Result<TaskItem, TaskItemRepositoryError>> CreateTaskAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Result<TaskItem, TaskItemRepositoryError>> UpdateTaskAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Page<TaskItem>> GetPageByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Page<TaskItem>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public abstract record TaskItemRepositoryError : IError;
