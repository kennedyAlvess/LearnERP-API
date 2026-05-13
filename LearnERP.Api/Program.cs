using LearnERP.Api.Middlewares;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<TenantContextMiddleware>();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

public partial class Program;
