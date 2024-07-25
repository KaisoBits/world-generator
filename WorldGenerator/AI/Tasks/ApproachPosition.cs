namespace WorldGenerator.AI;

public class ApproachPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly Position _targetPosition;

    public ApproachPosition(IScheduler scheduler, Position targetPosition)
    {
        _scheduler = scheduler;
        _targetPosition = targetPosition;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Position currentPos = _scheduler.Owner.Position;
        Position offset = _targetPosition - currentPos;

        if (offset == Position.Zero)
            return SchedulerTaskResult.Completed;

        Position newPos;
        if (Math.Abs(offset.X) > Math.Abs(offset.Y))
        {
            newPos = currentPos + new Position(offset.X / Math.Abs(offset.X), 0);
            World.Instance.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }
        else
        {
            newPos = currentPos + new Position(0, offset.Y / Math.Abs(offset.Y));
            World.Instance.ScheduleMoveEntity(_scheduler.Owner, newPos);
        }

        if (_targetPosition == newPos)
            return SchedulerTaskResult.Completed;

        return SchedulerTaskResult.Continue;
    }
}
