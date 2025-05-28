namespace Task_Manager.TaskManagement.Core.Entities;

public interface IUserData
{
    Guid Id { get; }
    string DisplayName { get; }
}

// projection of user from Identity micro-service
public sealed class User : IDomainModel<IUserData, User>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    // entire application unique identifier,
    // which controls under Identity micro-service
    public Guid Id { get; init; }
    public string DisplayName { get; private set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private User(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public static User Create(Guid id, string displayName)
    {
        return new User(id, displayName);
    }

    public static User ConvertFromData(IUserData userData)
    {
        return new User(userData.Id, userData.DisplayName);
    }
}
