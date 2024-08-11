using WorldGenerator.Factories;

namespace WorldGenerator.AI;

public class GoToNearestBuildingScheduler : Scheduler
{
    private readonly SchedulerTaskFactory _schedulerTaskFactory;

    public GoToNearestBuildingScheduler(SchedulerTaskFactory schedulerTaskFactory)
    {
        _schedulerTaskFactory = schedulerTaskFactory;
    }

    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return _schedulerTaskFactory.CreateTask<FindEntityPositionTask>(this).WithData(EntityType.Parse("*.building.*"), "targetPosition");
        yield return _schedulerTaskFactory.CreateTask<ApproachPositionTask>(this).WithData(Recall<Vector>("targetPosition")!);
    }
}
