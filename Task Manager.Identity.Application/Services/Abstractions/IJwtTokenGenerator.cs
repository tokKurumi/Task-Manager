using Task_Manager.Identity.Application.Services.Contracts;
using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Services.Abstractions;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(ApplicationUser user);
}
