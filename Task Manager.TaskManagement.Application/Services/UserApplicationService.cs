using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Application.Services.Contracts;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services;

public abstract record UserApplicationError : IError;

public interface IUserApplicationService
{
    public Task<Result<CreateUserResponse, UserApplicationError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
}

public class UserApplicationService(
    IUserRepository userRepository
) : IUserApplicationService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<CreateUserResponse, UserApplicationError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = User.Create(command.Id, command.DisplayName);

        return await _userRepository.CreateAsync(user, cancellationToken)
            .MapError(error => (UserApplicationError)new UserRepositoryInnerError(error))
            .Map(user => new CreateUserResponse(user));
    }
}

public sealed record UserRepositoryInnerError(UserRepositoryError InnerError) : UserApplicationError;
