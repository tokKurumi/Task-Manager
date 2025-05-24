using Task_Manager.Common;
using Task_Manager.Task.Application.Services.Abstractions;
using Task_Manager.Task.Application.Services.Contracts;
using Task_Manager.Task.Core.Entities;

namespace Task_Manager.Task.Application.Services;

public abstract record TaskApplicationError : IError;
public abstract record TaskCommentApplicationError : TaskApplicationError;

public interface ITaskApplicationService
{
    Task<Result<TaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<TaskItem?, TaskApplicationError>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskItem>, TaskApplicationError>> GetPageAsync(IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TaskItem, TaskApplicationError>> UpdateTaskAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task<Result<TaskApplicationError>> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> GetCommentByIdAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskCommentApplicationError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> UpdateCommentsync(TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskCommentApplicationError>> DeleteCommentAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
}

public class TaskApplicationService(
    ITaskItemRepository taskItemRepository,
    TimeProvider timeProvider
) : ITaskApplicationService
{
    private readonly ITaskItemRepository _taskItemRepository = taskItemRepository;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var createTaskItemResult = TaskItem.TryCreate(
            command.UserId,
            command.Title,
            command.Description,
            command.Notes,
            command.ApproximateCompletedAt,
            _timeProvider
        );
        if (createTaskItemResult.IsFailure)
        {
            return new TaskItemCreationError(createTaskItemResult.Error!);
        }

        var taskItemRepositoryCreate = await _taskItemRepository.CreateAsync(createTaskItemResult.Value!, cancellationToken);
        if (taskItemRepositoryCreate.IsFailure)
        {
            return new TaskRepositoryInnerError(taskItemRepositoryCreate.Error!);
        }

        return taskItemRepositoryCreate.Value!;
    }

    public async Task<Result<TaskItem?, TaskApplicationError>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.GetByIdAsync(id, cancellationToken);
        return result.MapError<TaskApplicationError>(error => new TaskRepositoryInnerError(error));
    }

    public async Task<Result<Page<TaskItem>, TaskApplicationError>> GetPageAsync(IPagination pagination, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.GetPageAsync(pagination, cancellationToken);
        return result.MapError<TaskApplicationError>(error => new TaskRepositoryInnerError(error));
    }

    public async Task<Result<TaskItem, TaskApplicationError>> UpdateTaskAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.UpdateAsync(taskItem, cancellationToken);
        return result.MapError<TaskApplicationError>(error => new TaskRepositoryInnerError(error));
    }

    public async Task<Result<TaskApplicationError>> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.DeleteAsync(id, cancellationToken);
        return result.MapError<TaskApplicationError>(error => new TaskRepositoryInnerError(error));
    }

    public async Task<Result<TaskComment, TaskCommentApplicationError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.AddCommentAsync(taskId, comment, cancellationToken);
        return result.MapError<TaskCommentApplicationError>(error => new TaskCommentRepositoryInnerError(error));
    }

    public async Task<Result<TaskComment, TaskCommentApplicationError>> GetCommentByIdAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.GetCommentByIdAsync(taskId, commentId, cancellationToken);
        return result.MapError<TaskCommentApplicationError>(error => new TaskCommentRepositoryInnerError(error));
    }

    public async Task<Result<Page<TaskComment>, TaskCommentApplicationError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.GetCommentsPageAsync(taskId, pagination, cancellationToken);
        return result.MapError<TaskCommentApplicationError>(error => new TaskCommentRepositoryInnerError(error));
    }

    public async Task<Result<TaskComment, TaskCommentApplicationError>> UpdateCommentsync(TaskComment comment, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.UpdateCommentAsync(comment, cancellationToken);
        return result.MapError<TaskCommentApplicationError>(error => new TaskCommentRepositoryInnerError(error));
    }

    public async Task<Result<TaskCommentApplicationError>> DeleteCommentAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default)
    {
        var result = await _taskItemRepository.DeleteCommentAsync(taskId, commentId, cancellationToken);
        return result.MapError<TaskCommentApplicationError>(error => new TaskCommentRepositoryInnerError(error));
    }
}

public sealed record TaskItemCreationError(TaskItemCreateError InnerError) : TaskApplicationError;

public sealed record TaskRepositoryInnerError(TaskItemRepositoryError InnerError) : TaskApplicationError;

public sealed record TaskCommentRepositoryInnerError(TaskItemCommentRepositoryError InnerError) : TaskCommentApplicationError;
