using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Core.Entities;

public sealed class TaskComment : IDomainModel
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; init; }
    public User Author { get; init; }
    public string Message { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TaskComment(User author, string message, DateTimeOffset timestamp)
    {
        Id = Guid.CreateVersion7();
        Author = author;
        Message = message;
        Timestamp = timestamp;
    }

    public static Result<TaskComment, TaskCommentCreateError> TryCreate(User author, string message, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return new EmptyMessageError();
        }

        return new TaskComment(author, message, timeProvider.GetUtcNow());
    }
}

public abstract record TaskCommentCreateError : IError;

public sealed record EmptyMessageError : TaskCommentCreateError;
