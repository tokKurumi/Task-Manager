using Task_Manager.Common;
using Task_Manager.Core.Entities;

namespace Task_Manager.Core.Abstractions;

public interface ITaskItemRepository
{
    Task<Result<TaskItem, TaskItemError>> CreateTaskAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Result<TaskItem, TaskItemError>> UpdateTaskAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Page<TaskItem>> GetPageByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Page<TaskItem>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
