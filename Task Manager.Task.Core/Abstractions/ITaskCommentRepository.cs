using Task_Manager.Common;
using Task_Manager.Core.Entities;

namespace Task_Manager.Core.Abstractions;

public interface ITaskCommentRepository
{
    Task<Result<TaskComment, TaskCommentError>> CreateCommentAsync(TaskComment comment, CancellationToken cancellationToken = default);
    Task<TaskComment?> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<Page<TaskComment>> GetPageByTaskIdAsync(Guid taskId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
