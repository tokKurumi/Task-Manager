using Task_Manager.Common;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Abstractions;

public interface ITaskItemRepository : IGenericRepository<TaskItem, TaskItemRepositoryError>
{
    Task<Result<TaskComment, TaskItemCommentRepositoryError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskItemCommentRepositoryError>> GetCommentPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
}

public abstract record TaskItemRepositoryError : RepositoryError;

public abstract record TaskItemCommentRepositoryError : TaskItemRepositoryError;
