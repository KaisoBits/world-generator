using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public class TraitFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TraitFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateTrait<T>() where T : ITrait
    {
        T newTrait = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return newTrait;
    }
}
