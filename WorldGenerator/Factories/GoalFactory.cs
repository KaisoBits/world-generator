using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.AI;

namespace WorldGenerator.Factories;

public class GoalFactory
{
    private readonly IServiceProvider _serviceProvider;

    public GoalFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateGoal<T>() where T : IGoal
    {
        T goal = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return goal;
    }
}
