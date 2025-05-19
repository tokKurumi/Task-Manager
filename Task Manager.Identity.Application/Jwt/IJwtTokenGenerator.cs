using Task_Manager.Identity.Core.Entities;

namespace Task_Manager.Identity.Application.Jwt;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(ApplicationUser user);
}
