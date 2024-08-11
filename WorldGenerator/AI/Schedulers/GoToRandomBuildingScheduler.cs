using WorldGenerator.Factories;

namespace WorldGenerator.AI;

public class GoToRandomBuildingScheduler : Scheduler
{
    private readonly SchedulerTaskFactory _schedulerTaskFactory;

    public GoToRandomBuildingScheduler(SchedulerTaskFactory schedulerTaskFactory)
    {
        _schedulerTaskFactory = schedulerTaskFactory;
    }

    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return _schedulerTaskFactory.CreateTask<FindRandomEntityPosition>(this).WithData(EntityType.Parse("*.building.*"), "targetPosition");
        yield return _schedulerTaskFactory.CreateTask<ApproachPositionTask>(this).WithData(Recall<Vector>("targetPosition")!);
    }
}
