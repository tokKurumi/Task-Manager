using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

public sealed class TaskItem
{
    private readonly Dictionary<Guid, TaskComment> _comments = [];

    public Guid Id { get; init; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Notes { get; private set; }
    public TaskItemStatus Status { get; init; }
    public IReadOnlyCollection<TaskComment> Comments => _comments.Values;

    private TaskItem(string title, string description, string notes, TaskItemStatus status)
    {
        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        Notes = notes;
        Status = status;
    }

    public static Result<TaskItem, TaskItemError> TryCreate(
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
            return new TaskItemStatusCreationError(statusCreateResult.Error!);
        }

        return new TaskItem(title, description, notes, statusCreateResult.Value!);
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

public abstract record TaskItemError : IError;

public sealed record EmptyTitleError : TaskItemError;

public sealed record EmptyDescriptionError : TaskItemError;

public sealed record TaskItemStatusCreationError(TaskItemStatusError InnerError) : TaskItemError;

public abstract record AddCommentError : IError;

public sealed record DuplicateCommentError(TaskComment Comment) : AddCommentError;
