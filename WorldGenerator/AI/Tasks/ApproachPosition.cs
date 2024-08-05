using System.Diagnostics.Metrics;
using WorldGenerator.States;

namespace WorldGenerator.AI;

public class ApproachPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly World _world;
    private Vector _targetPosition;

    private const int stepTime = 20;
    private int _counter = stepTime;

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

        int newCounterValue = _counter - (_scheduler.Owner.GetState<SpeedState>()?.Speed ?? 10);
        if (newCounterValue > 0)
        {
            _counter = newCounterValue;
            return SchedulerTaskResult.Continue;
        }

        _counter = stepTime;

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
