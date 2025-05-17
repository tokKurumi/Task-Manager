using Microsoft.Extensions.DependencyInjection;
using Task_Manager.Identity.Infrastructure.Mappers;

namespace Task_Manager.Identity.Infrastructure.Extensions;

public static class MappingExtensions
{
    public static IServiceCollection AddInfrastructureMappers(this IServiceCollection services)
    {
        return services
            .AddScoped<ApplicationUserMapper>();
    }
}
