using Task_Manager.Common;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Core.Abstractions;

public interface ITaskCommentRepository
{
    Task<Result<TaskComment, TaskCommentRepositoryError>> CreateCommentAsync(TaskComment comment, CancellationToken cancellationToken = default);
    Task<TaskComment?> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<Page<TaskComment>> GetPageByTaskIdAsync(Guid taskId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public abstract record TaskCommentRepositoryError(string Code) : Error(Code);
