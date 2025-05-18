using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Task_Manager.Identity.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        return builder
            .AddMediatr();
    }

    private static IHostApplicationBuilder AddMediatr(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediator();

        return builder;
    }
}
