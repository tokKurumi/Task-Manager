using Mediator;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services;
using Task_Manager.TaskManagement.Application.Services.Abstractions;

namespace Task_Manager.TaskManagement.Application.IntegrationEventHandlers.UserCreateEvent;

public sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IUserApplicationService userApplicationService
) : ICommandHandler<CreateUserCommand, Result<CreateUserResponse, UserApplicationError>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserApplicationService _userApplicationService = userApplicationService;

    public async ValueTask<Result<CreateUserResponse, UserApplicationError>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteWithStrategyAsync(async ct =>
        {
            return await _userApplicationService.CreateUserAsync(UserCreateCommandMapper.ToUserApplicationServiceModel(command), ct)
                .Map(UserCreateCommandMapper.ToUseCaseModel);
        }, cancellationToken);
    }
}
