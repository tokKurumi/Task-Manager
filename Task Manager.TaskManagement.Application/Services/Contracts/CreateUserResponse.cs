using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public sealed record CreateUserResponse(
    User User
);
