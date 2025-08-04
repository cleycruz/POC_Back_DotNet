using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CarritoComprasAPI.Adapters.Primary;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Commands.Carrito;
using CarritoComprasAPI.Core.Queries.Carrito;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.UnitTests.Application.Controllers;

public class CarritoControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CarritoController _controller;

    public CarritoControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _controller = new CarritoController(_mediatorMock.Object);
    }

    [Fact]
    public void Constructor_ConMediatorNulo_DeberiaLanzarArgumentNullException()
    {
        // Act & Assert
        Action act = () => new CarritoController(null!);
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("mediator");
    }

    [Fact]
    public async Task ObtenerCarrito_ConUsuarioIdValido_DeberiaRetornarCarritoDto()
    {
        // Arrange
        var usuarioId = "usuario123";
        var carrito = _fixture.Create<Carrito>();
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerCarritoPorUsuarioQuery>(), default))
                    .ReturnsAsync(carrito);

        // Act
        var result = await _controller.ObtenerCarrito(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<CarritoDto>();
        
        _mediatorMock.Verify(x => x.Send(It.Is<ObtenerCarritoPorUsuarioQuery>(q => q.UsuarioId == usuarioId), default), Times.Once);
    }

    [Fact]
    public async Task ObtenerCarrito_ConCarritoNulo_DeberiaRetornarCarritoVacio()
    {
        // Arrange
        var usuarioId = "usuario123";
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerCarritoPorUsuarioQuery>(), default))
                    .ReturnsAsync((Carrito)null!);

        // Act
        var result = await _controller.ObtenerCarrito(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var carritoDto = okResult.Value.Should().BeOfType<CarritoDto>().Subject;
        carritoDto.UsuarioId.Should().Be(usuarioId);
    }

    [Fact]
    public async Task ObtenerCarrito_ConArgumentException_DeberiaRetornarBadRequest()
    {
        // Arrange
        var usuarioId = "usuario123";
        var errorMessage = "Usuario ID inválido";
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerCarritoPorUsuarioQuery>(), default))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.ObtenerCarrito(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ObtenerCarrito_ConExcepcionGenerica_DeberiaRetornarError500()
    {
        // Arrange
        var usuarioId = "usuario123";
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerCarritoPorUsuarioQuery>(), default))
                    .ThrowsAsync(new Exception("Error interno"));

        // Act
        var result = await _controller.ObtenerCarrito(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error interno del servidor");
    }

    [Fact]
    public async Task ObtenerTotal_ConUsuarioIdValido_DeberiaRetornarTotal()
    {
        // Arrange
        var usuarioId = "usuario123";
        var totalEsperado = 150.50m;
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerTotalCarritoQuery>(), default))
                    .ReturnsAsync(totalEsperado);

        // Act
        var result = await _controller.ObtenerTotal(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(totalEsperado);
        
        _mediatorMock.Verify(x => x.Send(It.Is<ObtenerTotalCarritoQuery>(q => q.UsuarioId == usuarioId), default), Times.Once);
    }

    [Fact]
    public async Task ObtenerTotal_ConArgumentException_DeberiaRetornarBadRequest()
    {
        // Arrange
        var usuarioId = "usuario123";
        var errorMessage = "Usuario ID inválido";
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerTotalCarritoQuery>(), default))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.ObtenerTotal(usuarioId);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ObtenerCarrito_ConUsuarioIdInvalido_DeberiaLanzarArgumentException(string usuarioIdInvalido)
    {
        // Arrange
        _mediatorMock.Setup(x => x.Send(It.IsAny<ObtenerCarritoPorUsuarioQuery>(), default))
                    .ThrowsAsync(new ArgumentException("Usuario ID no puede estar vacío"));

        // Act
        var result = await _controller.ObtenerCarrito(usuarioIdInvalido);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
