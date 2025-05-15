using Task_Manager.Common;

namespace Task_Manager.Identity.Core.Entities;

public class ApplicationUser
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string DisplayName { get; init; }
    public string PasswordHash { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    private ApplicationUser(string email, string displayName, string passwordHash, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static Result<ApplicationUser, ApplicationUserError> TryCreate(string email, string displayName, string passwordHash, TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<ApplicationUser, ApplicationUserError>.Failure(new EmptyEmailError());
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result<ApplicationUser, ApplicationUserError>.Failure(new EmptyDisplayNameError());
        }

        if (string.IsNullOrEmpty(passwordHash))
        {
            return Result<ApplicationUser, ApplicationUserError>.Failure(new EmptyPasswordHashError());
        }

        return Result<ApplicationUser, ApplicationUserError>.Success(new ApplicationUser(email, displayName, passwordHash, timeProvider.GetUtcNow()));
    }
}

public abstract record ApplicationUserError(string Code) : Error(Code);

public sealed record EmptyEmailError() : ApplicationUserError("EmptyEmail");

public sealed record EmptyDisplayNameError() : ApplicationUserError("EmptyDisplayName");

public sealed record EmptyPasswordHashError() : ApplicationUserError("EmptyPasswordHash");
