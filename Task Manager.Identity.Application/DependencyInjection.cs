using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Application.UseCases.Auth.Login;
using Task_Manager.Identity.Application.UseCases.Auth.Register;

namespace Task_Manager.Identity.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        return builder
            .AddServices()
            .AddValiadtion()
            .AddMediatr();
    }

    private static IHostApplicationBuilder AddValiadtion(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Assembly).Assembly);

        return builder;
    }

    private static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();

        return builder;
    }

    private static IHostApplicationBuilder AddMediatr(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediator(config =>
        {
            config.ServiceLifetime = ServiceLifetime.Scoped;
        });

        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TracingBehavior<,>));
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

        builder.Services.Scan(scan =>
        {
            scan.FromAssemblyOf<Assembly>()
                .AddClasses(classes => classes.AssignableTo(typeof(IValidationErrorFactory<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });

        return builder;
    }
}

public class TracingBehavior<TMessage, TResponse>(
    ActivitySource activitySource
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    private readonly ActivitySource _activitySource = activitySource;

    public ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        using var activity = _activitySource.StartActivity($"Handling {typeof(TMessage).Name}");

        return next(message, cancellationToken);
    }
}

// TODO: think a way to generate IValidationErrorFactory for it's Scrutor registration
// Mb use Roslyn to generate it?
public class LoginUserValidationErrorFactory
    : IValidationErrorFactory<Result<LoginUserResponse, OneOfError<AuthError, ValidationError>>>
{
    public Result<LoginUserResponse, OneOfError<AuthError, ValidationError>> Create(ValidationError error)
        => new OneOfError<AuthError, ValidationError>(error);
}

public class RegisterUserValidationErrorFactory
    : IValidationErrorFactory<Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>>>
{
    public Result<RegisterUserResponse, OneOfError<AuthError, ValidationError>> Create(ValidationError error)
        => new OneOfError<AuthError, ValidationError>(error);
}
