using Riok.Mapperly.Abstractions;

namespace Task_Manager.Identity.Application.UseCases.Auth.Login;

[Mapper]
public static partial class LoginUserMapper
{
    public static partial Services.Contracts.LoginUserRequest ToAuthServiceModel(LoginUserRequest loginUserRequest);
    public static partial LoginUserResponse ToUseCaseModel(Services.Contracts.LoginUserResponse loginUserResult);
}
