﻿using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Core.Entities;

public interface ITaskCommentData
{
    Guid Id { get; }
    Guid AuthorId { get; }
    string Message { get; }
    DateTimeOffset Timestamp { get; }
}

public sealed class TaskComment : IDomainModel<ITaskCommentData, TaskComment>
{
    public Guid Id { get; init; }
    public Guid AuthorId { get; init; }
    public string Message { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    private TaskComment(User author, string message, DateTimeOffset timestamp)
    {
        Id = Guid.CreateVersion7();
        AuthorId = author.Id;
        Message = message;
        Timestamp = timestamp;
    }

    private TaskComment(Guid authorId, string message, DateTimeOffset timestamp)
    {
        AuthorId = authorId;
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

    public static TaskComment ConvertFromData(ITaskCommentData taskCommentData)
    {
        return new TaskComment(taskCommentData.AuthorId, taskCommentData.Message, taskCommentData.Timestamp);
    }
}

public abstract record TaskCommentCreateError : IError;

public sealed record EmptyMessageError : TaskCommentCreateError;
