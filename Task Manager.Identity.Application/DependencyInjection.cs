using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Task_Manager.Common;
using Task_Manager.Identity.Application.Services;
using Task_Manager.Identity.Application.Services.Abstractions;

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

        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

        builder.Services.AddValiadtionFactories();

        return builder;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>")]
    private static IServiceCollection AddValiadtionFactories(this IServiceCollection services)
    {
        // 1. Get all services which implements
        //      RequestClassHandlerWrapper<TCommand, Result<T, OneOfError<TError, ValidationError>>>
        //      CommandClassHandlerWrapper<TCommand, Result<T, OneOfError<TError, ValidationError>>>
        // 2. Get it's T and TError
        // 3. Register IValidationErrorFactory<Result<T, OneOfError<TError, ValidationError>>>

        List<(Type ServiceType, Type Implementation)> factoriesToRegister = [];
        foreach (var serviceDescriptor in services)
        {
            if (!serviceDescriptor.ServiceType.IsGenericType)
            {
                continue;
            }

            var typeDefinition = serviceDescriptor.ServiceType.GetGenericTypeDefinition();
            if (!(typeDefinition == typeof(RequestClassHandlerWrapper<,>) || typeDefinition == typeof(CommandClassHandlerWrapper<,>)))
            {
                continue;
            }

            var wrapperGenericArguments = serviceDescriptor.ServiceType.GetGenericArguments();
            if (wrapperGenericArguments.Length != 2
                || !wrapperGenericArguments[1].IsGenericType
                || wrapperGenericArguments[1].GetGenericTypeDefinition() != typeof(Result<,>)
            )
            {
                continue;
            }

            var wrapperResultGenericArguments = wrapperGenericArguments[1].GetGenericArguments();
            if (wrapperResultGenericArguments.Length != 2
                || !wrapperResultGenericArguments[1].IsGenericType
                || wrapperResultGenericArguments[1].GetGenericTypeDefinition() != typeof(OneOfError<,>)
            )
            {
                continue;
            }

            var wrapperOneOfErrorGenericArguments = wrapperResultGenericArguments[1].GetGenericArguments();
            if (wrapperOneOfErrorGenericArguments.Length != 2
                || wrapperOneOfErrorGenericArguments[1] != typeof(ValidationError)
            )
            {
                continue;
            }

            var tType = wrapperResultGenericArguments[0];
            var tErrorType = wrapperOneOfErrorGenericArguments[0];

            // Construct GenericValidationErrorFactory<tType, tErrorType>
            var factoryType = typeof(GenericValidationErrorFactory<,>).MakeGenericType(tType, tErrorType);
            var responseType = typeof(Result<,>).MakeGenericType(tType, typeof(OneOfError<,>).MakeGenericType(tErrorType, typeof(ValidationError)));
            var interfaceType = typeof(IValidationErrorFactory<>).MakeGenericType(responseType);

            factoriesToRegister.Add((interfaceType, factoryType));
        }

        foreach (var (serviceType, implementation) in factoriesToRegister)
        {
            services.TryAddSingleton(serviceType, implementation);
        }

        return services;
    }
}
