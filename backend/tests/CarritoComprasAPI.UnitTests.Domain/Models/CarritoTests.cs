using AutoFixture;
using FluentAssertions;
using Xunit;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Domain.Events.Carrito;
using CarritoComprasAPI.Core.Domain.ValueObjects;

namespace CarritoComprasAPI.UnitTests.Domain.Models;

public class CarritoTests
{

    [Fact]
    public void Crear_ConUsuarioIdValido_DeberiaCrearCarritoCorrectamente()
    {
        // Arrange
        var usuarioId = "usuario123";

        // Act
        var carrito = Carrito.Crear(usuarioId);

        // Assert
        carrito.Should().NotBeNull();
        carrito.UsuarioCarrito.Value.Should().Be(usuarioId);
        carrito.Items.Should().BeEmpty();
        carrito.Total.Should().Be(0);
        carrito.CantidadItems.Should().Be(0);
        carrito.CantidadProductos.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConUsuarioIdInvalido_DeberiaLanzarArgumentException(string usuarioIdInvalido)
    {
        // Act & Assert
        Action act = () => Carrito.Crear(usuarioIdInvalido);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgregarItem_ConProductoValido_DeberiaAgregarItemCorrectamente()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();
        var cantidad = 2;

        // Act
        carrito.AgregarItem(producto, cantidad);

        // Assert
        carrito.Items.Should().HaveCount(1);
        carrito.CantidadItems.Should().Be(1);
        carrito.CantidadProductos.Should().Be(cantidad);
        carrito.Total.Should().Be(producto.PrecioProducto.Value * cantidad);

        var item = carrito.Items[0];
        item.ProductoId.Should().Be(producto.Id);
        item.CantidadItem.Value.Should().Be(cantidad);
        item.PrecioUnitario.Value.Should().Be(producto.PrecioProducto.Value);
        item.Subtotal.Should().Be(producto.PrecioProducto.Value * cantidad);
    }

    [Fact]
    public void AgregarItem_ProductoExistente_DeberiaActualizarCantidad()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido(stock: 10);
        carrito.AgregarItem(producto, 2);

        // Act
        carrito.AgregarItem(producto, 3);

        // Assert
        carrito.Items.Should().HaveCount(1);
        carrito.CantidadProductos.Should().Be(5);
        carrito.Items[0].CantidadItem.Value.Should().Be(5);
    }

    [Fact]
    public void AgregarItem_ConProductoNulo_DeberiaLanzarArgumentNullException()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");

        // Act & Assert
        Action act = () => carrito.AgregarItem(null!, 1);
        act.Should().Throw<ArgumentNullException>().WithParameterName("producto");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgregarItem_ConCantidadInvalida_DeberiaLanzarArgumentException(int cantidadInvalida)
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();

        // Act & Assert
        Action act = () => carrito.AgregarItem(producto, cantidadInvalida);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgregarItem_SinStockSuficiente_DeberiaLanzarInvalidOperationException()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido(stock: 5);

        // Act & Assert
        Action act = () => carrito.AgregarItem(producto, 10);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Stock insuficiente*");
    }

    [Fact]
    public void ActualizarCantidadItem_ConCantidadValida_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido(stock: 10);
        carrito.AgregarItem(producto, 2);

        // Act
        carrito.ActualizarCantidadItem(producto.Id, 5);

        // Assert
        var item = carrito.Items[0];
        item.CantidadItem.Value.Should().Be(5);
        carrito.CantidadProductos.Should().Be(5);
    }

    [Fact]
    public void ActualizarCantidadItem_ConCantidadCero_DeberiaEliminarItem()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();
        carrito.AgregarItem(producto, 2);

        // Act
        carrito.ActualizarCantidadItem(producto.Id, 0);

        // Assert
        carrito.Items.Should().BeEmpty();
        carrito.CantidadItems.Should().Be(0);
    }

    [Fact]
    public void EliminarItem_ConProductoExistente_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();
        carrito.AgregarItem(producto, 2);

        // Act
        carrito.EliminarItem(producto.Id);

        // Assert
        carrito.Items.Should().BeEmpty();
        carrito.Total.Should().Be(0);
    }

    [Fact]
    public void Vaciar_ConItemsEnCarrito_DeberiaVaciarCompletamente()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto1 = CrearProductoValido(nombre: "Producto1", precio: 100);
        var producto2 = CrearProductoValido(nombre: "Producto2", precio: 200);
        carrito.AgregarItem(producto1, 1);
        carrito.AgregarItem(producto2, 2);

        // Act
        carrito.Vaciar();

        // Assert
        carrito.Items.Should().BeEmpty();
        carrito.Total.Should().Be(0);
        carrito.CantidadItems.Should().Be(0);
        carrito.CantidadProductos.Should().Be(0);
    }

    [Fact]
    public void TieneItems_ConItemsEnCarrito_DeberiaRetornarTrue()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();
        carrito.AgregarItem(producto, 1);

        // Act & Assert
        carrito.TieneItems().Should().BeTrue();
    }

    [Fact]
    public void TieneItems_ConCarritoVacio_DeberiaRetornarFalse()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");

        // Act & Assert
        carrito.TieneItems().Should().BeFalse();
    }

    [Fact]
    public void ObtenerItem_ConProductoExistente_DeberiaRetornarItem()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");
        var producto = CrearProductoValido();
        carrito.AgregarItem(producto, 2);

        // Act
        var item = carrito.ObtenerItem(producto.Id);

        // Assert
        item.Should().NotBeNull();
        item!.ProductoId.Should().Be(producto.Id);
        item.CantidadItem.Value.Should().Be(2);
    }

    [Fact]
    public void ObtenerItem_ConProductoNoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var carrito = Carrito.Crear("usuario123");

        // Act
        var item = carrito.ObtenerItem(999);

        // Assert
        item.Should().BeNull();
    }

    private static Producto CrearProductoValido(string nombre = "Producto Test", decimal precio = 100, int stock = 10)
    {
        return Producto.Crear(nombre, "Descripción test", precio, stock, "Categoria Test");
    }
}
