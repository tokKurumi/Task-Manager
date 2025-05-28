using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Core.Entities;

public enum TaskStatus
{
    Created,
    InProgress,
    Completed,
}

public interface ITaskItemStatusData
{
    TaskStatus Status { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? ApproximateCompletedAt { get; }
    DateTimeOffset? CompletedAt { get; }
}

public sealed class TaskItemStatus : IDomainModel<ITaskItemStatusData, TaskItemStatus>
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

    private TaskItemStatus(
        TaskStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset? approximateCompletedAt,
        DateTimeOffset? completedAt
    )
    {
        Status = status;
        CreatedAt = createdAt;
        ApproximateCompletedAt = approximateCompletedAt;
        CompletedAt = completedAt;
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

    public static TaskItemStatus ConvertFromData(ITaskItemStatusData data)
    {
        return new TaskItemStatus(data.Status, data.CreatedAt, data.ApproximateCompletedAt, data.CompletedAt);
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
