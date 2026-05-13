using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LearnERP.Tests;

public sealed class ProblemDetailsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task PostValidation_ReturnsProblemDetails()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/v1/problem-details-test/validation", new { Name = string.Empty });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetException_ReturnsProblemDetails()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/v1/problem-details-test/exception");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
