using System.Security.Claims;
using LearnERP.Api.Authentication;

namespace LearnERP.Api.Middlewares;

public sealed class TenantContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is not null)
        {
            var allowAnonymous = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() is not null;
            if (!allowAnonymous && context.User.Identity?.IsAuthenticated == true)
            {
                var tenantId = context.User.FindFirstValue(JwtClaimConstants.TenantId);
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Missing tenant_id claim.");
                    return;
                }

                var accessor = context.RequestServices.GetRequiredService<ITenantContextAccessor>();
                if (accessor is TenantContextAccessor concreteAccessor)
                {
                    concreteAccessor.SetTenant(tenantId);
                }
            }
        }

        await _next(context);
    }
}
