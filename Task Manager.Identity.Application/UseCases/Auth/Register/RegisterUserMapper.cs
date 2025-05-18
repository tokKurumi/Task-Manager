using Riok.Mapperly.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Register;

[Mapper]
public static partial class RegisterUserMapper
{
    public static partial Core.Contracts.RegisterUserCommand ToCore(RegisterUserCommand registerUserCommand);

    public static partial RegisterUserResponse ToApplication(Core.Contracts.RegisterUserResponse registerUserResult);
}
