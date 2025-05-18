using Task_Manager.Common;

namespace Task_Manager.Identity.Core.Entities;

public interface IUserData
{
    Guid Id { get; }
    string Email { get; }
    string DisplayName { get; }
    DateTimeOffset CreatedAt { get; }
}

public class ApplicationUser
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string DisplayName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    private ApplicationUser(string email, string displayName, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        DisplayName = displayName;
        CreatedAt = createdAt;
    }

    private ApplicationUser(Guid id, string email, string displayName, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        DisplayName = displayName;
        CreatedAt = createdAt;
    }

    public static Result<ApplicationUser, ApplicationUserError> TryCreate(string email, string displayName, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return new EmptyEmailError();
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return new EmptyDisplayNameError();
        }

        return new ApplicationUser(email, displayName, timeProvider.GetUtcNow());
    }

    public static Result<ApplicationUser, ApplicationUserError> TryCreate(IUserData userData, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(userData.Email))
        {
            return new EmptyEmailError();
        }

        if (string.IsNullOrWhiteSpace(userData.DisplayName))
        {
            return new EmptyDisplayNameError();
        }

        return new ApplicationUser(userData.Id, userData.Email, userData.DisplayName, userData.CreatedAt);
    }
}

public abstract record ApplicationUserError(string Code) : Error(Code);

public sealed record EmptyEmailError() : ApplicationUserError("EmptyEmail");

public sealed record EmptyDisplayNameError() : ApplicationUserError("EmptyDisplayName");

public sealed record EmptyPasswordHashError() : ApplicationUserError("EmptyPasswordHash");
