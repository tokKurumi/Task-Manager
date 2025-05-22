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

    public static Result<TaskItemStatus, TaskItemStatusError> TryCreate(DateTimeOffset? approximateCompletedAt, TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();
        if (approximateCompletedAt.HasValue && approximateCompletedAt.Value <= now)
        {
            return new ApproximateCompletedAtInPastError();
        }

        return new TaskItemStatus(now, approximateCompletedAt);
    }

    public Result<InProgressTaskItemStatusError> TryMoveToInProgress()
    {
        if (Status != TaskStatus.InProgress)
        {
            return new AlreadyInProgressError();
        }

        Status = TaskStatus.InProgress;
        CompletedAt = null;

        return Result<InProgressTaskItemStatusError>.Success();
    }

    public Result<ComplitionTaskItemStatusError> TryComplete(TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();
        if (now < CreatedAt)
        {
            return new CompletedAtInPastError();
        }

        Status = TaskStatus.Completed;
        CompletedAt = now;

        return Result<ComplitionTaskItemStatusError>.Success();
    }
}

public abstract record TaskItemStatusError : IError;

public sealed record ApproximateCompletedAtInPastError : TaskItemStatusError;

public abstract record InProgressTaskItemStatusError : IError;

public sealed record AlreadyInProgressError : InProgressTaskItemStatusError;

public abstract record ComplitionTaskItemStatusError : IError;

public sealed record CompletedAtInPastError : ComplitionTaskItemStatusError;
