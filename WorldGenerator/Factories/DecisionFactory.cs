using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public class DecisionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DecisionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateDecision<T>() where T : IDecision
    {
        T newDecision = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return newDecision;
    }
}
