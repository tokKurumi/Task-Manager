using Riok.Mapperly.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

[Mapper]
public static partial class RegisterUserMapper
{
    public static partial Services.Contracts.RegisterUserCommand ToAuthServiceModel(RegisterUserCommand registerUserCommand);

    public static partial RegisterUserResponse ToUseCaseModel(Services.Contracts.RegisterUserResponse registerUserResult);
}
