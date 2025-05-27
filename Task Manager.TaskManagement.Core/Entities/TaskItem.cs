using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Core.Entities;

public interface ITaskItemData
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Title { get; }
    public string Description { get; }
    public string Notes { get; }
    public ITaskItemStatusData Status { get; }
}

public sealed class TaskItem : IDomainModel, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly Dictionary<Guid, TaskComment> _comments = [];

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Notes { get; private set; }
    public TaskItemStatus Status { get; init; }
    public IReadOnlyCollection<TaskComment> Comments => _comments.Values;
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TaskItem(User user, string title, string description, string notes, TaskItemStatus status)
    {
        Id = Guid.CreateVersion7();
        UserId = user.Id;
        Title = title;
        Description = description;
        Notes = notes;
        Status = status;
    }

    private TaskItem(Guid id, Guid userId, string title, string description, string notes, TaskItemStatus status)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Notes = notes;
        Status = status;
    }

    public static Result<TaskItem, TaskItemCreateError> TryCreate(
        User user,
        string title,
        string description,
        string notes,
        DateTimeOffset? approximateCompletedAt,
        TimeProvider timeProvider
    )
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return new EmptyTitleError();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return new EmptyDescriptionError();
        }

        var statusCreateResult = TaskItemStatus.TryCreate(approximateCompletedAt, timeProvider);
        if (statusCreateResult.IsFailure)
        {
            return new StatusCreateError(statusCreateResult.Error!);
        }

        return new TaskItem(user, title, description, notes, statusCreateResult.Value!);
    }

    public static Result<TaskItem, TaskItemCreateError> TryConvertFromData(ITaskItemData taskItemData)
    {
        if (string.IsNullOrWhiteSpace(taskItemData.Title))
        {
            return new EmptyTitleError();
        }

        if (string.IsNullOrWhiteSpace(taskItemData.Description))
        {
            return new EmptyDescriptionError();
        }

        var statusCreateResult = TaskItemStatus.TryConvertFromData(taskItemData.Status);
        if (statusCreateResult.IsFailure)
        {
            return new StatusCreateError(statusCreateResult.Error!);
        }

        return new TaskItem(
            taskItemData.Id,
            taskItemData.UserId,
            taskItemData.Title,
            taskItemData.Description,
            taskItemData.Notes,
            statusCreateResult.Value!
        );
    }

    public Result<AddCommentError> TryAddComment(TaskComment comment)
    {
        if (_comments.ContainsKey(comment.Id))
        {
            return new DuplicateCommentError(comment);
        }

        _comments.Add(comment.Id, comment);

        return Result<AddCommentError>.Success();
    }
}

public abstract record TaskItemCreateError : IError;

public sealed record InvalidUserIdError : TaskItemCreateError;

public sealed record EmptyTitleError : TaskItemCreateError;

public sealed record EmptyDescriptionError : TaskItemCreateError;

public sealed record StatusCreateError(TaskItemStatusCreateError InnerError) : TaskItemCreateError;

public abstract record AddCommentError : IError;

public sealed record DuplicateCommentError(TaskComment Comment) : AddCommentError;
