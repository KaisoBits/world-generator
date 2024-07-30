namespace WorldGenerator.AI;

public class GoToNearestBuildingScheduler : Scheduler
{
    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return new FindEntityPosition(this, Layer.Buildings, "targetPosition");
        yield return new ApproachPosition(this, (Vector)Recall("targetPosition")!);
    }
}
