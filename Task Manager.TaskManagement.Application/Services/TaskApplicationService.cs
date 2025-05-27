using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Application.Services.Contracts;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services;

public abstract record TaskApplicationError : IError;
public abstract record TaskCommentApplicationError : TaskApplicationError;

public interface ITaskApplicationService
{
    Task<Result<UserTaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<UserTaskItem?, TaskApplicationError>> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Page<UserTaskItem>, TaskApplicationError>> GetTaskPageAsync(IPagination pagination, CancellationToken cancellationToken = default);
    Task<Result<UserTaskItem, TaskApplicationError>> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<TaskApplicationError>> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default);
    Task<Result<TaskComment, TaskApplicationError>> AddCommentAsync(AddCommentCommand command, CancellationToken cancellationToken = default);
    Task<Result<Page<TaskComment>, TaskApplicationError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default);
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

    public async Task<Result<UserTaskItem, TaskApplicationError>> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(command.UserPerformerId, cancellationToken)
            .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
            .EnsureNotNull(() => new UserNotFoundError(command.UserPerformerId))
            .BindAsync(user => TaskItem.TryCreate(
                    user,
                    command.Title,
                    command.Description,
                    command.Notes,
                    command.ApproximateCompletedAt,
                    _timeProvider
                )
                .MapError(error => (TaskApplicationError)new TaskItemCreationError(error))
                .ToTask()
                .BindAsync(task => _taskItemRepository.CreateAsync(task, cancellationToken)
                    .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
                    .Map(taskEntity => new UserTaskItem(user, taskEntity))
                )
            );
    }

    public async Task<Result<UserTaskItem?, TaskApplicationError>> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _taskItemRepository.GetByIdAsync(id, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
            .BindAsync(taskItem => taskItem is null
                ? Result<UserTaskItem?, TaskApplicationError>.Success(null).ToTask()
                : _userRepository.GetByIdAsync(taskItem.UserId, cancellationToken)
                    .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
                    .EnsureNotNull(() => new UserNotFoundError(taskItem.UserId))
                    .Map(user => (UserTaskItem?)new UserTaskItem(user, taskItem))
            );
    }

    public async Task<Result<Page<UserTaskItem>, TaskApplicationError>> GetTaskPageAsync(IPagination pagination, CancellationToken cancellationToken = default)
    {
        async Task<Result<Page<UserTaskItem>, TaskApplicationError>> FetchUsersAndMapItems(
            Page<TaskItem> taskPage,
            CancellationToken cancelationToken
        )
        {
            var userIds = taskPage.Select(task => task.UserId).Distinct().ToList();
            return await _userRepository
                .GetUsersByIds(userIds, cancelationToken)
                .MapError(error => (TaskApplicationError)new UsersNotFoundError(error.UserIds))
                .Map(users =>
                {
                    var userMap = users.ToDictionary(user => user.Id, user => user);
                    var userTaskItems = taskPage.Items.Select(taskItem => new UserTaskItem(
                        userMap[taskItem.UserId],
                        taskItem
                    )).ToList();
                    return new Page<UserTaskItem>(userTaskItems, taskPage.PageNumber, taskPage.PageSize, taskPage.Count);
                });
        }

        return await _taskItemRepository
            .GetPageAsync(pagination, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
            .BindAsync(taskPage => taskPage.Count == 0
                ? Result<Page<UserTaskItem>, TaskApplicationError>.Success(new Page<UserTaskItem>([], pagination, 0)).ToTask()
                : FetchUsersAndMapItems(taskPage, cancellationToken)
            );
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
                .Map(user => new UserTaskItem(user, taskItem))
            );
    }

    public async Task<Result<TaskApplicationError>> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        return await _taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
        .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
        .EnsureNotNull(() => new TaskNotFoundError(command.TaskId))
        .Ensure(
            taskItem => taskItem.UserId == command.UserPerformerId,
            taskItem => new UserTaskOwnershipDeniedError(taskItem.Id, taskItem.UserId, command.UserPerformerId)
        )
        .EnsureAsync(
            taskItem => _userRepository.ExistsAsync(taskItem.UserId, cancellationToken),
            taskItem => new UserNotFoundError(taskItem.UserId)
        )
        .TapAsync(taskItem => _taskItemRepository.DeleteAsync(taskItem.Id, cancellationToken))
        .ToUnitResult();
    }

    public async Task<Result<TaskComment, TaskApplicationError>> AddCommentAsync(AddCommentCommand command, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(command.UserPerformerId, cancellationToken)
            .MapError(error => (TaskApplicationError)new UserRepositoryInnerError(error))
            .EnsureNotNull(() => new UserNotFoundError(command.UserPerformerId))
            .BindAsync(user => _taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
                .MapError(error => (TaskApplicationError)new TaskRepositoryInnerError(error))
                .EnsureNotNull(() => new TaskNotFoundError(command.TaskId))
                .BindAsync(task => TaskComment.TryCreate(user, command.Message, _timeProvider)
                    .MapError(error => (TaskApplicationError)new TaskCommentCreationError(error))
                    .EnsureBy(comment => task.TryAddComment(comment), error => new TaskCommentAddError(error))
                    .ToTask()
                )
            );
    }

    public async Task<Result<Page<TaskComment>, TaskApplicationError>> GetCommentsPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default)
    {
        return await _taskItemRepository.GetCommentPageAsync(taskId, pagination, cancellationToken)
            .MapError(error => (TaskApplicationError)new TaskCommentRepositoryInnerError(error));
    }
}

public sealed record UserNotFoundError(Guid UserId) : TaskApplicationError;

public sealed record UsersNotFoundError(IEnumerable<Guid> UserIds) : TaskApplicationError;

public sealed record UserTaskOwnershipDeniedError(Guid TaskId, Guid TaskUserOwnerId, Guid UserPerformerId) : TaskApplicationError;

public sealed record TaskNotFoundError(Guid TaskId) : TaskApplicationError;

public sealed record TaskItemCreationError(TaskItemCreateError InnerError) : TaskApplicationError;

public sealed record TaskRepositoryInnerError(TaskItemRepositoryError InnerError) : TaskApplicationError;

public sealed record TaskCommentRepositoryInnerError(TaskItemCommentRepositoryError InnerError) : TaskApplicationError;

public sealed record UserRepositoryInnerError(UserRepositoryError InnerError) : TaskApplicationError;

public sealed record TaskCommentCreationError(TaskCommentCreateError InnerError) : TaskApplicationError;

public sealed record TaskCommentAddError(AddCommentError InnerError) : TaskApplicationError;
