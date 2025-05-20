using FluentValidation;

namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

public record LoginUserRequest(
    string Email,
    string Password
);

public class LoginUserRequestValiadtor : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValiadtor()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email should not be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password should not be empty");
    }
}
