using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.AI;

namespace WorldGenerator.Factories;

public class SchedulerTaskFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SchedulerTaskFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateTask<T>(IScheduler scheduler) where T : ISchedulerTask
    {
        T newTrait = ActivatorUtilities.CreateInstance<T>(_serviceProvider, scheduler);
        return newTrait;
    }
}
