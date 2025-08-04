using AutoFixture;
using FluentAssertions;
using Xunit;

namespace CarritoComprasAPI.TestUtilities.Tests;

public class SampleUtilitiesTests
{
    [Fact]
    public void AutoFixture_DebeEstarConfiguraroCorrectamente()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var randomString = fixture.Create<string>();
        var randomInt = fixture.Create<int>();

        // Assert
        randomString.Should().NotBeNullOrEmpty();
        randomInt.Should().NotBe(0);
    }

    [Fact]
    public void Bogus_DebeGenerarDatos_Consistentes()
    {
        // Arrange
        var faker = new Bogus.Faker("es");

        // Act
        var name = faker.Person.FullName;
        var email = faker.Person.Email;

        // Assert
        name.Should().NotBeNullOrEmpty();
        email.Should().NotBeNullOrEmpty();
        email.Should().Contain("@");
    }
}
