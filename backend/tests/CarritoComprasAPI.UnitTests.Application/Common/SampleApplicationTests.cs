using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarritoComprasAPI.UnitTests.Application.Common;

public class SampleApplicationTests
{
    private readonly IFixture _fixture;

    public SampleApplicationTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void AutoMoq_DebeCrearMocks_Correctamente()
    {
        // Arrange
        var mockService = _fixture.Freeze<Mock<ISampleService>>();
        mockService.Setup(x => x.GetValue()).Returns("test");

        // Act
        var service = _fixture.Create<Mock<ISampleService>>();
        var result = service.Object.GetValue();

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void AutoFixture_DebeGenerarDatos_Automaticamente()
    {
        // Arrange & Act
        var randomString = _fixture.Create<string>();
        var randomInt = _fixture.Create<int>();

        // Assert
        randomString.Should().NotBeNullOrEmpty();
        randomInt.Should().NotBe(0);
    }
}

public interface ISampleService
{
    string GetValue();
}
