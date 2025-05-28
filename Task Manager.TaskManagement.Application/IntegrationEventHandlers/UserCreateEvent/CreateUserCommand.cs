using Mediator;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services;
using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.IntegrationEventHandlers.UserCreateEvent;

public sealed record CreateUserCommand(
    Guid Id,
    string DisplayName
) : ICommand<Result<CreateUserResponse, UserApplicationError>>;

public sealed record CreateUserResponse(
    User User
);
