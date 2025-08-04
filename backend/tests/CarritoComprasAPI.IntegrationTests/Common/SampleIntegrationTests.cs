using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace CarritoComprasAPI.IntegrationTests.Common;

public class SampleIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SampleIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ApplicationStart_DebeEstar_Disponible()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/");

        // Assert
        response.Should().NotBeNull();
        // No necesariamente debe ser OK, pero debe responder
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Redirect);
    }

    [Fact]
    public void WebApplicationFactory_DebeCrearse_Correctamente()
    {
        // Arrange & Act & Assert
        _client.Should().NotBeNull();
        _client.BaseAddress.Should().NotBeNull();
    }
}
