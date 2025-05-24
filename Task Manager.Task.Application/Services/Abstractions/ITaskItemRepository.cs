using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Abstractions;

public interface ITaskItemRepository : IGenericRepository<TaskItem, TaskItemRepositoryError>
{
    Task<Result<TaskComment, TaskItemCommentRepositoryError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskItemCommentRepositoryError>> GetCommentByIdAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskItemCommentRepositoryError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TaskItemCommentRepositoryError>> DeleteCommentAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
}

public abstract record TaskItemRepositoryError : IError;

public abstract record TaskItemCommentRepositoryError : TaskItemRepositoryError;
