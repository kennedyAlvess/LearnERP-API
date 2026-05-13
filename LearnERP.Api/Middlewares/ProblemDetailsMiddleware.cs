using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace LearnERP.Api.Middlewares;

public sealed class ProblemDetailsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno.",
                Type = "https://httpstatuses.com/500",
                Detail = "Ocorreu um erro inesperado.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
