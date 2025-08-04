using FluentAssertions;
using Xunit;

namespace CarritoComprasAPI.AcceptanceTests.Common;

public class SampleAcceptanceTests
{
    [Fact]
    public void Sample_AcceptanceTest_DebeEjecutarse()
    {
        // Arrange
        var expected = "acceptance test";

        // Act
        var result = "acceptance test";

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Framework_SpecFlow_DebeEstarDisponible()
    {
        // Arrange
        var specFlowAssembly = typeof(TechTalk.SpecFlow.StepDefinitionAttribute).Assembly;

        // Act & Assert
        specFlowAssembly.Should().NotBeNull();
        specFlowAssembly.GetName().Name.Should().Be("TechTalk.SpecFlow");
    }
}
