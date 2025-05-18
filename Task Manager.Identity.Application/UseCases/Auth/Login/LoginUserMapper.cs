using Riok.Mapperly.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

[Mapper]
public static partial class LoginUserMapper
{
    public static partial Core.Contracts.LoginUserRequest ToCore(LoginUserRequest loginUserRequest);
    public static partial LoginUserResponse ToApplication(Core.Contracts.LoginUserResponse loginUserResult);
}
