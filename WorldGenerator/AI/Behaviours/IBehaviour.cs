namespace WorldGenerator.AI;

public interface IBehaviour
{
    IScheduler? DetermineScheduler(IEntity entitiy);
}
