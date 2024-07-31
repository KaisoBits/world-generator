using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.AI;

namespace WorldGenerator.Factories;

public class SchedulerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SchedulerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateScheduler<T>() where T : IScheduler
    {
        T newTrait = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return newTrait;
    }
}
