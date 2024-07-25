using WorldGenerator.Schedulers;

namespace WorldGenerator.Behaviours;

public interface IBehaviour
{
    IScheduler? DetermineScheduler(IEntity entitiy);
}
