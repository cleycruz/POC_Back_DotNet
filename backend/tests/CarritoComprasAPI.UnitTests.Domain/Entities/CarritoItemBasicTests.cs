using FluentAssertions;
using Xunit;
using CarritoComprasAPI.Models;

namespace CarritoComprasAPI.UnitTests.Domain.Entities;

public class CarritoItemBasicTests
{
    [Fact]
    public void Constructor_ConDatosBasicos_DebeCrearCarritoItem()
    {
        // Arrange & Act
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = 3,
            PrecioUnitario = 25.00m
        };

        // Assert
        carritoItem.Id.Should().Be(1);
        carritoItem.CarritoId.Should().Be(100);
        carritoItem.ProductoId.Should().Be(200);
        carritoItem.Cantidad.Should().Be(3);
        carritoItem.PrecioUnitario.Should().Be(25.00m);
        carritoItem.FechaAgregado.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(1, 10.00, 10.00)]
    [InlineData(2, 25.50, 51.00)]
    [InlineData(5, 15.99, 79.95)]
    [InlineData(10, 100.00, 1000.00)]
    public void Subtotal_ConDiferentesCantidadesYPrecios_DebeCalcularCorrectamente(
        int cantidad, decimal precioUnitario, decimal subtotalEsperado)
    {
        // Arrange
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario
        };

        // Act
        var subtotal = carritoItem.Subtotal;

        // Assert
        subtotal.Should().Be(subtotalEsperado);
    }

    [Fact]
    public void ActualizarCantidad_ConCantidadValida_DebeActualizarCorrectamente()
    {
        // Arrange
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = 2,
            PrecioUnitario = 25.00m
        };

        // Act
        carritoItem.ActualizarCantidad(5);

        // Assert
        carritoItem.Cantidad.Should().Be(5);
    }

    [Fact]
    public void ActualizarPrecio_ConPrecioValido_DebeActualizarCorrectamente()
    {
        // Arrange
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = 2,
            PrecioUnitario = 25.00m
        };

        var nuevoPrecio = 30.50m;

        // Act
        carritoItem.ActualizarPrecio(nuevoPrecio);

        // Assert
        carritoItem.PrecioUnitario.Should().Be(nuevoPrecio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ActualizarCantidad_ConCantidadInvalida_DebeLanzarExcepcion(int cantidadInvalida)
    {
        // Arrange
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = 2,
            PrecioUnitario = 25.00m
        };

        // Act & Assert
        var act = () => carritoItem.ActualizarCantidad(cantidadInvalida);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void ActualizarPrecio_ConPrecioInvalido_DebeLanzarExcepcion(decimal precioInvalido)
    {
        // Arrange
        var carritoItem = new CarritoItem
        {
            Id = 1,
            CarritoId = 100,
            ProductoId = 200,
            Cantidad = 2,
            PrecioUnitario = 25.00m
        };

        // Act & Assert
        var act = () => carritoItem.ActualizarPrecio(precioInvalido);
        act.Should().Throw<ArgumentException>();
    }
}
