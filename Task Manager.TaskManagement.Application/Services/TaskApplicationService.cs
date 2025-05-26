using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Application.Services.Contracts;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services;

public abstract record TaskApplicationError : IError;
public abstract record TaskCommentApplicationError : TaskApplicationError;

public interface ITaskApplicationService
{
    Task<Result<TaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<UserTaskItem?, TaskApplicationError>> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Page<UserTaskItem>, TaskApplicationError>> GetTaskPageAsync(IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<UserTaskItem, TaskApplicationError>> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<TaskApplicationError>> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> GetCommentByIdAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskCommentApplicationError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskCommentApplicationError>> UpdateCommentsync(TaskComment comment, CancellationToken cancellationToken = default);
    Task<Result<TaskCommentApplicationError>> DeleteCommentAsync(Guid taskId, Guid commentId, CancellationToken cancellationToken = default);
}

public partial class TaskApplicationService(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    TimeProvider timeProvider
) : ITaskApplicationService
{
    private readonly ITaskItemRepository _taskItemRepository = taskItemRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(command.UserPerformerId, cancellationToken)
            .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
            .EnsureNotNull(() => new UserNotFoundError(command.UserPerformerId))
            .Bind(user => TaskItem.TryCreate
                (
                    user.Id,
                    command.Title,
                    command.Description,
                    command.Notes,
                    command.ApproximateCompletedAt,
                    _timeProvider
                )
                .MapError(error => (TaskApplicationError)new TaskItemCreationError(error))
            )
            .BindAsync(taskItem => _taskItemRepository.CreateAsync(taskItem, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error)));
    }

    public async Task<Result<UserTaskItem?, TaskApplicationError>> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _taskItemRepository.GetByIdAsync(id, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
            .BindAsync(taskItem => taskItem is null
                ? Task.FromResult(Result<UserTaskItem?, TaskApplicationError>.Success(null))
                : _userRepository.GetByIdAsync(taskItem.UserId, cancellationToken)
                    .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
                    .EnsureNotNull(() => new UserNotFoundError(taskItem.UserId))
                    .Map(user => (UserTaskItem?)new UserTaskItem(user, taskItem))
            );
    }

    public async Task<Result<Page<UserTaskItem>, TaskApplicationError>> GetTaskPageAsync(IPagination pagination, CancellationToken cancellationToken = default)
    {
        var taskItemResults = await _taskItemRepository.GetPageAsync(pagination, cancellationToken);
        if (taskItemResults.IsFailure)
        {
            return new TaskRepositoryInnerError(taskItemResults.Error!);
        }

        if (taskItemResults.Value!.Count == 0)
        {
            return Result<Page<UserTaskItem>, TaskApplicationError>.Success(new Page<UserTaskItem>([], pagination, 0));
        }

        var userIds = taskItemResults.Value.Items.Select(task => task.UserId).Distinct().ToList();
        var userResults = await _userRepository.GetUsersByIds(userIds, cancellationToken);
        if (userResults.IsFailure)
        {
            return new UsersNotFoundError(userResults.Error!.UserIds);
        }

        var userMap = userResults.Value!.ToDictionary(user => user.Id, user => user);
        var userTaskItems = taskItemResults.Value.Items.Select(taskItem => new UserTaskItem(
            userMap[taskItem.UserId],
            taskItem
        )).ToList();

        return new Page<UserTaskItem>(userTaskItems, pagination, taskItemResults.Value.Count);
    }

    public async Task<Result<UserTaskItem, TaskApplicationError>> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default)
    {
        return await _taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
            .EnsureNotNull(() => new TaskNotFoundError(command.TaskId))
            .Ensure(
                taskItem => taskItem.UserId == command.UserPerformerId,
                taskItem => new UserTaskOwnershipDeniedError(taskItem.Id, taskItem.UserId, command.UserPerformerId)
            )
            .BindAsync(taskItem => _userRepository.GetByIdAsync(command.UserPerformerId, cancellationToken)
            .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
            .EnsureNotNull(() => new UserNotFoundError(taskItem.UserId))
            .Map(user => new UserTaskItem(user, taskItem)));
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

public sealed record UserNotFoundError(Guid UserId) : TaskApplicationError;

public sealed record UsersNotFoundError(IEnumerable<Guid> UserIds) : TaskApplicationError;

public sealed record TaskOwnerNotFoundError(Guid TaskId) : TaskApplicationError;

public sealed record UserTaskOwnershipDeniedError(Guid TaskId, Guid TaskUserOwnerId, Guid UserPerformerId) : TaskApplicationError;

public sealed record TaskNotFoundError(Guid TaskId) : TaskApplicationError;

public sealed record TaskItemCreationError(TaskItemCreateError InnerError) : TaskApplicationError;

public sealed record TaskRepositoryInnerError(TaskItemRepositoryError InnerError) : TaskApplicationError;

public sealed record TaskCommentRepositoryInnerError(TaskItemCommentRepositoryError InnerError) : TaskCommentApplicationError;

public sealed record UserRepositoryInnerError(UserRepositoryError InnerError) : TaskApplicationError;
