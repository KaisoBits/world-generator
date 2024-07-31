using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.Moodlets;

namespace WorldGenerator.Factories;

public class MoodletFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MoodletFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateMoodlet<T>(int expireOn) where T : Moodlet
    {
        T newTrait = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        newTrait.ExpireOn = expireOn;
        return newTrait;
    }
}
