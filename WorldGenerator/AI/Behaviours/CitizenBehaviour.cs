namespace WorldGenerator.AI;

// PURPOSE: I want to get to the closest city
public class CitizenBehaviour : IBehaviour
{
    public IScheduler? DetermineScheduler(IEntity entitiy)
    {
        if (Random.Shared.Next(1, 11) == 1)
        {
            if (entitiy.InCondition(Condition.IN_BUILDING))
            {
                return new GoToRandomBuildingScheduler();
            }
            else
            {
                return new GoToNearestBuildingScheduler();
            }
        }
        else
        {
            return null;
        }
    }
}
