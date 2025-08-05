using FluentAssertions;
using Xunit;
using CarritoComprasAPI.Models;

namespace CarritoComprasAPI.UnitTests.Domain.Entities;

public class CarritoBasicTests
{
    [Fact]
    public void Constructor_ConDatosBasicos_DebeCrearCarrito()
    {
        // Arrange & Act
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        // Assert
        carrito.Id.Should().Be(1);
        carrito.UsuarioId.Should().Be("user123");
        carrito.Items.Should().NotBeNull();
        carrito.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EstaVacio_ConCarritoNuevo_DebeRetornarTrue()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        // Act & Assert
        carrito.EstaVacio.Should().BeTrue();
    }

    [Fact]
    public void Total_ConCarritoVacio_DebeRetornarCero()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        // Act & Assert
        carrito.Total.Should().Be(0);
    }

    [Fact]
    public void CantidadTotalItems_ConCarritoVacio_DebeRetornarCero()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        // Act & Assert
        carrito.CantidadTotalItems.Should().Be(0);
    }

    [Fact]
    public void LimpiarCarrito_ConItemsAgregados_DebeVaciarCarrito()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = 10
        };

        carrito.AgregarItem(producto, 2);

        // Act
        carrito.LimpiarCarrito();

        // Assert
        carrito.Items.Should().BeEmpty();
        carrito.EstaVacio.Should().BeTrue();
        carrito.Total.Should().Be(0);
    }

    [Fact]
    public void AgregarItem_ConProductoValido_DebeAgregarCorrectamente()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = 10
        };

        // Act
        carrito.AgregarItem(producto, 2);

        // Assert
        carrito.Items.Should().HaveCount(1);
        carrito.Total.Should().Be(100.00m);
        carrito.CantidadTotalItems.Should().Be(2);
        carrito.EstaVacio.Should().BeFalse();
    }

    [Fact]
    public void CalcularDescuento_ConDescuentoValido_DebeCalcularCorrectamente()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 100.00m,
            Stock = 10
        };

        carrito.AgregarItem(producto, 1);

        // Act
        var descuento = carrito.CalcularDescuento(10m);

        // Assert
        descuento.Should().Be(10.00m);
    }

    [Fact]
    public void CalcularTotalConDescuento_ConDescuentoValido_DebeCalcularCorrectamente()
    {
        // Arrange
        var carrito = new Carrito
        {
            Id = 1,
            UsuarioId = "user123"
        };

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 100.00m,
            Stock = 10
        };

        carrito.AgregarItem(producto, 1);

        // Act
        var totalConDescuento = carrito.CalcularTotalConDescuento(20m);

        // Assert
        totalConDescuento.Should().Be(80.00m);
    }
}
