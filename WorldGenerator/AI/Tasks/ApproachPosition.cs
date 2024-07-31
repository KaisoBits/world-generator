namespace WorldGenerator.AI;

public class ApproachPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly World _world;
    private Vector _targetPosition;

    public ApproachPosition(IScheduler scheduler, World world)
    {
        _scheduler = scheduler;
        _world = world;
    }

    public ApproachPosition WithData(Vector targetPosition)
    {
        _targetPosition = targetPosition;

        return this;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Vector currentPos = _scheduler.Owner.Position;
        Vector offset = _targetPosition - currentPos;

        if (offset == Vector.Zero)
            return SchedulerTaskResult.Completed;

        Vector newPos;
        if (Math.Abs(offset.X) > Math.Abs(offset.Y))
        {
            newPos = currentPos + new Vector(offset.X / Math.Abs(offset.X), 0);
            _world.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }
        else
        {
            newPos = currentPos + new Vector(0, offset.Y / Math.Abs(offset.Y));
            _world.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }

        if (_targetPosition == newPos)
            return SchedulerTaskResult.Completed;

        return SchedulerTaskResult.Continue;
    }
}
