using WorldGenerator.AI.Tasks;

namespace WorldGenerator.AI;

public class GoToRandomBuildingScheduler : Scheduler
{
    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return new FindRandomEntityPosition(this, Layer.Buildings, "targetPosition");
        yield return new ApproachPosition(this, (Vector)Recall("targetPosition")!);
    }
}
