using Microsoft.Extensions.DependencyInjection;
using Task_Manager.Identity.Infrastructure.Mappers;

namespace Task_Manager.Identity.Infrastructure.Extensions;

internal static class MappingExtensions
{
    internal static IServiceCollection AddInfrastructureMappers(this IServiceCollection services)
    {
        return services
            .AddScoped<ApplicationUserMapper>();
    }
}
