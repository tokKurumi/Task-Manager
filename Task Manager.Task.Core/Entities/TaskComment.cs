using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

public class TaskComment
{
    public Guid Id { get; init; }
    public User Author { get; init; }
    public string Message { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    private TaskComment(User author, string message, DateTimeOffset timestamp)
    {
        Id = Guid.CreateVersion7();
        Author = author;
        Message = message;
        Timestamp = timestamp;
    }

    public static Result<TaskComment, TaskCommentError> TryCreate(User author, string message, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Result<TaskComment, TaskCommentError>.Failure(new EmptyMessageError());
        }

        return Result<TaskComment, TaskCommentError>.Success(new TaskComment(author, message, timeProvider.GetUtcNow()));
    }
}

public abstract record TaskCommentError(string Code) : Error(Code);

public sealed record EmptyMessageError() : TaskCommentError("EmptyMessage");
