using Microsoft.Extensions.DependencyInjection;

namespace WorldGenerator.Factories;

public class IntentResolverFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IntentResolverFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateResolver<T>() where T : IIntentResolver
    {
        T newResolver = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return newResolver;
    }
}
