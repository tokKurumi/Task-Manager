using Task_Manager.Common;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public interface ITaskItemRepository : IGenericRepository<TaskItem, TaskItemRepositoryError>
{
    Task<Result<TaskItem, TaskItemRepositoryError>> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskItemRepositoryError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskItemRepositoryError>> GetCommentPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
}

public abstract record TaskItemRepositoryError : RepositoryError;

public sealed record TaskNotFoundError(Guid TaskId) : TaskItemRepositoryError;
