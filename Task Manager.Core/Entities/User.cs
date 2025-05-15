using Task_Manager.Common;

namespace Task_Manager.Core.Entities;

public class User
{
    private readonly List<TaskItem> _tasks = [];

    public Guid Id { get; init; }
    public string Email { get; init; }
    public string FullName { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private User(string email, string fullName, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        FullName = fullName;
        CreatedAt = createdAt;
    }

    public static Result<User, UserError> TryCreate(string email, string fullName, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<User, UserError>.Failure(new EmptyEmailError());
        }

        if (string.IsNullOrEmpty(fullName))
        {
            return Result<User, UserError>.Failure(new EmptyFullNameError());
        }

        return Result<User, UserError>.Success(new User(email, fullName, timeProvider.GetUtcNow()));
    }

    public Result<TaskItem, UserError> TryAddTask(string title, string description, TimeProvider timeProvider)
    {
        var taskResult = TaskItem.TryCreate(this, title, description, timeProvider);
        if (taskResult.IsFailure)
        {
            return Result<TaskItem, UserError>.Failure(new UserTaskError(taskResult.Error!));
        }

        var task = taskResult.Value!;
        _tasks.Add(task);

        return Result<TaskItem, UserError>.Success(task);
    }
}

public abstract record UserError(string Code) : Error(Code);

public sealed record EmptyEmailError() : UserError("EmptyEmail");

public sealed record EmptyFullNameError() : UserError("EmptyFullName");

public sealed record UserTaskError(TaskItemError InnerError) : UserError($"Task.{InnerError.Code}");
