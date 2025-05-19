using Riok.Mapperly.Abstractions;
using Task_Manager.Identity.IdentityAPI.Controllers.Auth.Models;

using AuthUseCase = Task_Manager.Identity.Application.UseCases.Auth;

namespace Task_Manager.Identity.IdentityAPI.Controllers.Auth;

[Mapper]
public static partial class AuthMapper
{
    public static partial AuthUseCase.Register.RegisterUserCommand ToUseCase(RegisterUserRequest request);
    public static partial AuthUseCase.Login.LoginUserRequest ToUseCase(LoginUserRequest request);
}
