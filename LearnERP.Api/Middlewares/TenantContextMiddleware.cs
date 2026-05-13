namespace LearnERP.Api.Middlewares;

public sealed class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Placeholder para resolução de TenantContext em issues futuras.
        await _next(context);
    }
}
