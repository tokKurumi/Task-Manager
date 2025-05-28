using Riok.Mapperly.Abstractions;

namespace Task_Manager.TaskManagement.Application.IntegrationEventHandlers.UserCreateEvent;

[Mapper]
public static partial class UserCreateCommandMapper
{
    public static partial Services.Contracts.CreateUserCommand ToUserApplicationServiceModel(CreateUserCommand createUserCommand);
    public static partial CreateUserResponse ToUseCaseModel(Services.Contracts.CreateUserResponse createUserResponse);
}
