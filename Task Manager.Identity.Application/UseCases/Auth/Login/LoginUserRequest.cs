using FluentValidation;
using Mediator;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password
) : IRequest<Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>>;

public sealed record LoginUserResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email should not be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password should not be empty");
    }
}
