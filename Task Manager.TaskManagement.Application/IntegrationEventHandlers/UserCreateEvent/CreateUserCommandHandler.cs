using Mediator;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services;

namespace Task_Manager.TaskManagement.Application.IntegrationEventHandlers.UserCreateEvent;

public sealed class CreateUserCommandHandler(
    IUserApplicationService userApplicationService
) : ICommandHandler<CreateUserCommand, Result<CreateUserResponse, UserApplicationError>>
{
    private readonly IUserApplicationService _userApplicationService = userApplicationService;

    public async ValueTask<Result<CreateUserResponse, UserApplicationError>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        return await _userApplicationService.CreateUserAsync(UserCreateCommandMapper.ToUserApplicationServiceModel(command), cancellationToken)
            .Map(UserCreateCommandMapper.ToUseCaseModel);
    }
}
