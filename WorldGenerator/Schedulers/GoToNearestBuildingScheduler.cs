using WorldGenerator.Tasks;

namespace WorldGenerator.Schedulers;

public class GoToNearestBuildingScheduler : Scheduler
{
    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return new FindNearestPathTo(this, Layer.Buildings, "targetPosition");
        yield return new ApproachPosition(this, (Position)Recall("targetPosition")!);
    }
}
