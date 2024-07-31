using WorldGenerator.AI.Tasks;
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
        yield return _schedulerTaskFactory.CreateTask<FindRandomEntityPosition>(this).WithData(Layer.Buildings, "targetPosition");
        yield return _schedulerTaskFactory.CreateTask<ApproachPosition>(this).WithData((Vector)Recall("targetPosition")!);
    }
}
