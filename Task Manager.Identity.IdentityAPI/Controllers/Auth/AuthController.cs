using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth;

[ApiController]
[ApiVersion(1.0)]
[Route("[controller]")]
public class AuthController(
    ISender sender
) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(AuthMapper.ToUseCase(command), cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.Value.Match(
                authError => MapAuthError(authError),
                validationError => new BadRequestObjectResult(validationError.InnerFailures)
            )
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(AuthMapper.ToUseCase(request), cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.Value.Match(
                authError => MapAuthError(authError),
                validationError => new BadRequestObjectResult(validationError.InnerFailures)
            )
        );
    }

    private static IActionResult MapAuthError(AuthError error)
    {
        return error switch
        {
            UserAlreadyExistError alreadyExistError => new ConflictObjectResult($"User with {alreadyExistError.Email} does already exist."),
            CreationUserError creationError => new BadRequestObjectResult(creationError.InnerError),
            RepositoryCreateUserError repositoryCreateError => new BadRequestObjectResult(repositoryCreateError.InnerError),
            PasswordPolicyValidationResult passwordPolicyError => new BadRequestObjectResult(passwordPolicyError.InnerError),
            UserNotFoundError notFoundError => new NotFoundObjectResult($"User with {notFoundError.Email} does not exist."),
            InvalidPasswordError invalidPasswordError => new UnauthorizedObjectResult($"Invalid password for user {invalidPasswordError.Email}."),
            _ => throw new Exception("") // TODO: think about handling unexpected errors
        };
    }
}
