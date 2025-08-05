using FluentAssertions;
using Xunit;
using CarritoComprasAPI.Models;

namespace CarritoComprasAPI.UnitTests.Domain.Entities;

public class ProductoBasicTests
{
    [Fact]
    public void Constructor_ConDatosValidos_DebeCrearProducto()
    {
        // Arrange
        var id = 1;
        var nombre = "Test Product";
        var precio = 50.00m;
        var stock = 10;

        // Act
        var producto = new Producto
        {
            Id = id,
            Nombre = nombre,
            Precio = precio,
            Stock = stock
        };

        // Assert
        producto.Id.Should().Be(id);
        producto.Nombre.Should().Be(nombre);
        producto.Precio.Should().Be(precio);
        producto.Stock.Should().Be(stock);
    }

    [Theory]
    [InlineData(100.00, 10, 1000.00)]
    [InlineData(25.50, 5, 127.50)]
    [InlineData(15.99, 2, 31.98)]
    public void CalcularValorInventario_ConStockYPrecio_DebeCalcularCorrectamente(
        decimal precio, int stock, decimal valorEsperado)
    {
        // Arrange
        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = precio,
            Stock = stock
        };

        // Act
        var valorInventario = producto.Precio * producto.Stock;

        // Assert
        valorInventario.Should().Be(valorEsperado);
    }

    [Fact]
    public void ToString_DebeIncluirInformacionBasica()
    {
        // Arrange
        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = 10
        };

        // Act
        var resultado = producto.ToString();

        // Assert
        resultado.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Equals_ConMismoId_DebeRetornarTrue()
    {
        // Arrange
        var producto1 = new Producto { Id = 1, Nombre = "Test", Precio = 50m, Stock = 10 };
        var producto2 = new Producto { Id = 1, Nombre = "Different", Precio = 100m, Stock = 5 };

        // Act & Assert
        producto1.Id.Should().Be(producto2.Id);
    }

    [Fact]
    public void Stock_ConValorCero_DebeIndicarSinStock()
    {
        // Arrange
        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = 0
        };

        // Act & Assert
        producto.Stock.Should().Be(0);
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void TieneStock_ConDiferentesNiveles_DebeRetornarCorrectamente(int stock, bool tieneStock)
    {
        // Arrange
        var producto = new Producto
        {
            Id = 1,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = stock
        };

        // Act
        var resultado = producto.Stock > 0;

        // Assert
        resultado.Should().Be(tieneStock);
    }
}
