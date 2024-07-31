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
        yield return _schedulerTaskFactory.CreateTask<FindEntityPositionTask>(this).WithData(Layer.Buildings, "targetPosition");
        yield return _schedulerTaskFactory.CreateTask<ApproachPosition>(this).WithData(Recall<Vector>("targetPosition")!);
    }
}
