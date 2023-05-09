using Flaggy;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace FlaggyApi;

public class FeatureGateAttribute : ActionFilterAttribute
{
    private readonly string _featureFlagKey;

    public FeatureGateAttribute(string featureFlagKey)
    {
        _featureFlagKey = featureFlagKey;
    }
    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ldClient = context.HttpContext.RequestServices.GetRequiredService<LdClient>();
        var ldContext = BuildMultiContext(context);
        var enabled = ldClient.BoolVariation(_featureFlagKey, ldContext, false);
        
        if (enabled)
            await next();
        else
            context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
    }

    private static Context BuildMultiContext(ActionExecutingContext context)
    {
        var userAgent = context.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
        var deviceContext = Context.Builder(FeatureFlags.CreateContextKey(userAgent))
            .Set("user-agent", userAgent)
            .Kind("device")
            .Build();
        var userContext = Context.Builder(FeatureFlags.CreateContextKey("1"))
            .Build();
        var ldContext = Context.MultiBuilder().Add(deviceContext).Add(userContext).Build();
        return ldContext;
    }
}
