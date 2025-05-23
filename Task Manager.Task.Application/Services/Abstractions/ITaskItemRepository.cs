using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services.Abstractions;

public interface ITaskItemRepository : IGenericRepository<TaskItem, TaskItemRepositoryError>
{
    Task<Result<TaskComment, TaskCommentCreateError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentCreateError>> GetCommentByIdAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskCommentCreateError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TaskCommentCreateError>> DeleteCommentAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
}

public abstract record TaskItemRepositoryError : IError;
