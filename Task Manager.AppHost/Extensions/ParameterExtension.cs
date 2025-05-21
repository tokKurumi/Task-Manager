using Task_Manager.Integrations;

namespace Task_Manager.AppHost.Extensions;

public static class ParameterExtension
{
    public record JwtParameters(
        IResourceBuilder<ParameterResource> JwtSecret,
        IResourceBuilder<ParameterResource> JwtIssuer,
        IResourceBuilder<ParameterResource> JwtAudience,
        IResourceBuilder<ParameterResource> JwtAccessTokenLifetime
    );

    public static JwtParameters AddJwtParameters(this IDistributedApplicationBuilder builder)
    {
        const string configurationPath = "JWT";

        var jwtSecret = builder.AddParameterFromConfiguration($"{nameof(JwtOptions.SecretKey)}", $"{configurationPath}:{nameof(JwtOptions.SecretKey)}", secret: true);
        var jwtIssuer = builder.AddParameter($"{nameof(JwtOptions.Issuer)}", IdentityProject.API);
        var jwtAudience = builder.AddParameterFromConfiguration($"{nameof(JwtOptions.Audience)}", $"{configurationPath}:{nameof(JwtOptions.Audience)}");
        var jwtAccessTokenLifetime = builder.AddParameterFromConfiguration($"{nameof(JwtOptions.AccessTokenLifetime)}", $"{configurationPath}:{nameof(JwtOptions.AccessTokenLifetime)}");

        return new(jwtSecret, jwtIssuer, jwtAudience, jwtAccessTokenLifetime);
    }

    public static IResourceBuilder<T> WithJwtParameters<T>(this IResourceBuilder<T> builder, JwtParameters parameters)
        where T : IResourceWithEnvironment
    {
        return builder
            .WithEnvironment(JwtIntegration.JwtSecretKeyEnvironment, parameters.JwtSecret)
            .WithEnvironment(JwtIntegration.JwtIssuerEnvironment, parameters.JwtIssuer)
            .WithEnvironment(JwtIntegration.JwtAudienceEnvironment, parameters.JwtAudience)
            .WithEnvironment(JwtIntegration.JwtAccessTokenLifetimeEnvironment, parameters.JwtAccessTokenLifetime);
    }
}
