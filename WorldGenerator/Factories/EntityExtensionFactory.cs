using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.EntityExtensions;

namespace WorldGenerator.Factories;

public class EntityExtensionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EntityExtensionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateExtension<T>() where T : IEntityExtension
    {
        T newTrait = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return newTrait;
    }
}
