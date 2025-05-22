using Task_Manager.Common;

namespace Task_Manager.Task.Core.Entities;

// projection of user from Identity micro-service
public sealed class User
{
    private readonly Dictionary<Guid, TaskItem> _tasks = [];

    // entire application unique identifier,
    // which controls under Identity micro-service
    public Guid Id { get; init; }
    public string DisplayName { get; private set; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.Values;

    public User(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public Result<UserAddTaskError> TryAddTask(TaskItem task)
    {
        if (_tasks.ContainsKey(task.Id))
        {
            return new DuplicateTaskError(task);
        }

        _tasks.Add(task.Id, task);

        return Result<UserAddTaskError>.Success();
    }
}

public abstract record UserAddTaskError : IError;

public sealed record DuplicateTaskError(TaskItem Task) : UserAddTaskError;
