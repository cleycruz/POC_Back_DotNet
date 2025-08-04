using AutoFixture;
using AutoFixture.AutoMoq;

namespace CarritoComprasAPI.TestUtilities.Helpers;

public static class FixtureHelper
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        
        // Configuraciones específicas para evitar problemas con recursión
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }

    public static T Create<T>(this IFixture fixture) where T : class
    {
        return fixture.Create<T>();
    }

    public static List<T> CreateMany<T>(this IFixture fixture, int count = 3) where T : class
    {
        return fixture.CreateMany<T>(count).ToList();
    }
}
