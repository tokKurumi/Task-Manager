using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Core.Entities;

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

    private TaskItem(Guid userId, string title, string description, string notes, TaskItemStatus status)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Title = title;
        Description = description;
        Notes = notes;
        Status = status;
    }

    public static Result<TaskItem, TaskItemCreateError> TryCreate(
        Guid userId,
        string title,
        string description,
        string notes,
        DateTimeOffset? approximateCompletedAt,
        TimeProvider timeProvider
    )
    {
        if (userId == Guid.Empty)
        {
            return new InvalidUserIdError();
        }

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

        return new TaskItem(userId, title, description, notes, statusCreateResult.Value!);
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
