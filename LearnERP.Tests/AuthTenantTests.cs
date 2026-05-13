using System.Net;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LearnERP.Api.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LearnERP.Tests;

public sealed class AuthTenantTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetProtectedEndpoint_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/v1/auth/ping");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProtectedEndpoint_WithTokenMissingTenantId_Returns403()
    {
        var client = _factory.CreateClient();
        var token = CreateToken(includeTenantId: false);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/v1/auth/ping");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProtectedEndpoint_WithTenantId_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var token = CreateToken(includeTenantId: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/v1/auth/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private string CreateToken(bool includeTenantId)
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(JwtOptions.SectionName);
        var secret = section.GetValue<string>("Secret") ?? "dev-secret-change-me";
        var issuer = section.GetValue<string>("Issuer") ?? "LearnERP";
        var audience = section.GetValue<string>("Audience") ?? "LearnERP.Api";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "test-user"),
            new(JwtClaimConstants.Role, "Admin")
        };

        if (includeTenantId)
        {
            claims.Add(new Claim(JwtClaimConstants.TenantId, "tenant-test"));
        }

        return JwtTestTokenFactory.CreateToken(secret, issuer, audience, claims);
    }
}
