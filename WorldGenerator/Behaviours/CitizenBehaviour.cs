using WorldGenerator.Schedulers;

namespace WorldGenerator.Behaviours;

// PURPOSE: I want to get to the closest city
public class CitizenBehaviour : IBehaviour
{
    public IScheduler? DetermineScheduler(IEntity entitiy)
    {
        if (entitiy.InCondition(Condition.IN_BUILDING))
            return null;

        if (Random.Shared.Next(1, 11) == 1)
            return new GoToNearestBuildingScheduler();
        else 
            return null;
    }
}
