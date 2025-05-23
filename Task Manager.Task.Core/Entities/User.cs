using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

// projection of user from Identity micro-service
public sealed class User : IAggregateRoot
{
    // entire application unique identifier,
    // which controls under Identity micro-service
    public Guid Id { get; init; }
    public string DisplayName { get; private set; }

    private User(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public static Result<User, UserCreateError> TryCreate(Guid id, string displayName)
    {
        return new User(id, displayName);
    }
}

public abstract record UserCreateError : IError;

public abstract record UserAddTaskError : IError;

public sealed record DuplicateTaskError(TaskItem Task) : UserAddTaskError;
