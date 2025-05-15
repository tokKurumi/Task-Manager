using Task_Manager.Common;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Identity.Infrastructure.Entities;

namespace Task_Manager.Identity.Infrastructure.Mappers;

public class ApplicationUserMapper(
    TimeProvider timeProvider
)
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public Result<ApplicationUser, ApplicationUserError> TryMapToDomainModel(UserEntity user)
    {
        return ApplicationUser.TryCreate(user, _timeProvider);
    }

    public static UserEntity MapToInfrastructureModel(ApplicationUser user)
    {
        return new UserEntity(
            user.Id,
            user.Email,
            user.DisplayName,
            user.PasswordHash,
            user.CreatedAt
        );
    }
}
