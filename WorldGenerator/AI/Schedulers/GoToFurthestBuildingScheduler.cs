namespace WorldGenerator.AI;

public class GoToFurthestBuildingScheduler : Scheduler
{
    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return new FindEntityPosition(this, Layer.Buildings, "targetPosition", true);
        yield return new ApproachPosition(this, (Position)Recall("targetPosition")!);
    }
}
