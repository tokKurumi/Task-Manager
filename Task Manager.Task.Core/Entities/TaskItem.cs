using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

public enum TaskStatus
{
    Created,
    InProgress,
    Completed,
}

public class TaskItem
{
    private readonly List<TaskComment> _comments = [];

    public Guid Id { get; init; }
    public User Author { get; init; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyCollection<TaskComment> Comments => _comments.AsReadOnly();

    private TaskItem(User author, string title, string description, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        Author = author;
        Title = title;
        Description = description;
        Status = TaskStatus.Created;
        CreatedAt = createdAt;
    }

    public static Result<TaskItem, TaskItemError> TryCreate(User author, string title, string description, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<TaskItem, TaskItemError>.Failure(new EmptyTitleError());
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<TaskItem, TaskItemError>.Failure(new EmptyDescriptionError());
        }

        return Result<TaskItem, TaskItemError>.Success(new TaskItem(author, title, description, timeProvider.GetUtcNow()));
    }

    public Result<TaskComment, TaskItemError> TryCreateComment(User author, string message, TimeProvider timeProvider)
    {
        var commentResult = TaskComment.TryCreate(author, message, timeProvider);
        if (commentResult.IsFailure)
        {
            return Result<TaskComment, TaskItemError>.Failure(new CommentError(commentResult.Error!));
        }

        var comment = commentResult.Value!;
        _comments.Add(comment);

        return Result<TaskComment, TaskItemError>.Success(comment);
    }
}

public abstract record TaskItemError(string Code) : Error(Code);

public sealed record EmptyTitleError() : TaskItemError("EmptyTitle");

public sealed record EmptyDescriptionError() : TaskItemError("EmptyDescription");

public sealed record CommentError(TaskCommentError InnerError) : TaskItemError($"Comment.{InnerError.Code}");