using FluentValidation;
using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password
) : ICommand<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>;

public sealed record RegisterUserResponse(
    ApplicationUser User,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
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
