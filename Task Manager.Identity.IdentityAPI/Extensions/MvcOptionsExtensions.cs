using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Task_Manager.Identity.IdentityAPI.Extensions;

public static class MvcOptionsExtensions
{
    public static void UseRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
    {
        opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
    }

    public static void UseRoutePrefix(this MvcOptions opts, [StringSyntax("Route")] string prefix)
    {
        opts.UseRoutePrefix(new RouteAttribute(prefix));
    }
}

public class RoutePrefixConvention(
    IRouteTemplateProvider route
) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(route);

    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(controller => controller.Selectors))
        {
            if (selector.AttributeRouteModel is not null)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                continue;
            }

            selector.AttributeRouteModel = _routePrefix;
        }
    }
}