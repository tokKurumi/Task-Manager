using FluentValidation;

namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

public record RegisterUserRequest(
    string Email,
    string DisplayName,
    string Password
);

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email should not be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name should not be empty.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password should not be empty")
            .MinimumLength(8).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.");
    }
}
