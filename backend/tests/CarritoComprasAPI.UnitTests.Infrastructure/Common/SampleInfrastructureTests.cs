using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarritoComprasAPI.UnitTests.Infrastructure.Common;

public class SampleInfrastructureTests : IDisposable
{
    private readonly TestDbContext _context;

    public SampleInfrastructureTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
    }

    [Fact]
    public void InMemoryDatabase_DebeCrearse_Correctamente()
    {
        // Arrange & Act
        var canConnect = _context.Database.CanConnect();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public void TestEntity_DebeAgregarseCorrectamente()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Value = 42 };

        // Act
        _context.TestEntities.Add(entity);
        _context.SaveChanges();

        // Assert
        var retrievedEntity = _context.TestEntities.FirstOrDefault();
        retrievedEntity.Should().NotBeNull();
        retrievedEntity.Name.Should().Be("Test");
        retrievedEntity.Value.Should().Be(42);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}
