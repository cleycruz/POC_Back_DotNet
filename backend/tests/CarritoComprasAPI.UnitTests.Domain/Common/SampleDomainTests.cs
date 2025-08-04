using FluentAssertions;
using Xunit;

namespace CarritoComprasAPI.UnitTests.Domain.Common;

public class SampleDomainTests
{
    [Fact]
    public void SampleTest_DebeEjecutarse_Correctamente()
    {
        // Arrange
        var valorEsperado = 42;

        // Act
        var resultado = 21 * 2;

        // Assert
        resultado.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(2, 3, 5)]
    [InlineData(-1, 1, 0)]
    public void Suma_ConDosNumeros_DebeRetornarSumaCorrecta(int a, int b, int esperado)
    {
        // Arrange & Act
        var resultado = a + b;

        // Assert
        resultado.Should().Be(esperado);
    }
}
