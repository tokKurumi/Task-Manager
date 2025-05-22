using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

public enum TaskStatus
{
    Created,
    InProgress,
    Completed,
}

public sealed class TaskItemStatus
{
    public TaskStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ApproximateCompletedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private TaskItemStatus(DateTimeOffset createdAt, DateTimeOffset? approximateCompletedAt)
    {
        Status = TaskStatus.Created;
        CreatedAt = createdAt;
        ApproximateCompletedAt = approximateCompletedAt;
    }

    public static Result<TaskItemStatus, TaskItemStatusCreateError> TryCreate(DateTimeOffset? approximateCompletedAt, TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();
        if (approximateCompletedAt.HasValue && approximateCompletedAt.Value <= now)
        {
            return new ApproximateCompletedAtInPastError();
        }

        return new TaskItemStatus(now, approximateCompletedAt);
    }

    public Result<MoveToInProgeressError> TryMoveToInProgress()
    {
        if (Status != TaskStatus.InProgress)
        {
            return new AlreadyInProgressError();
        }

        Status = TaskStatus.InProgress;
        CompletedAt = null;

        return Result<MoveToInProgeressError>.Success();
    }

    public Result<CompleteError> TryComplete(TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();
        if (now < CreatedAt)
        {
            return new CompletedAtInPastError();
        }

        Status = TaskStatus.Completed;
        CompletedAt = now;

        return Result<CompleteError>.Success();
    }
}

public abstract record TaskItemStatusCreateError : IError;

public sealed record ApproximateCompletedAtInPastError : TaskItemStatusCreateError;

public abstract record MoveToInProgeressError : IError;

public sealed record AlreadyInProgressError : MoveToInProgeressError;

public abstract record CompleteError : IError;

public sealed record CompletedAtInPastError : CompleteError;
