﻿using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

public sealed class TaskComment
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
