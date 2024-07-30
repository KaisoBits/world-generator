namespace WorldGenerator.AI;

public class ApproachPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly Vector _targetPosition;

    public ApproachPosition(IScheduler scheduler, Vector targetPosition)
    {
        _scheduler = scheduler;
        _targetPosition = targetPosition;
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
            World.Instance.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }
        else
        {
            newPos = currentPos + new Vector(0, offset.Y / Math.Abs(offset.Y));
            World.Instance.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }

        if (_targetPosition == newPos)
            return SchedulerTaskResult.Completed;

        return SchedulerTaskResult.Continue;
    }
}
