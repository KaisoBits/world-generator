namespace WorldGenerator.AI;

public class FindRandomEntityPosition : ISchedulerTask
{
    private readonly World _world;
    private readonly IScheduler _scheduler;
    private EntityType _entityType = default!;
    private string _positionMemory = default!;

    public FindRandomEntityPosition(World world, IScheduler parent)
    {
        _world = world;
        _scheduler = parent;
    }

    public FindRandomEntityPosition WithData(EntityType entityType, string positionMemory)
    {
        _entityType = entityType;
        _positionMemory = positionMemory;

        return this;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Vector pos = _scheduler.Owner.Position;

        ITileView[] tiles = _world
            .Where(t => t.Contents.Any(e => e.EntityType.Matches(_entityType))).ToArray();

        if (tiles is [])
            return SchedulerTaskResult.Failed;

        ITileView target = Random.Shared.GetItems(tiles, 1)[0];

        _scheduler.Remember(_positionMemory, target.Position);

        return SchedulerTaskResult.Completed;
    }
}
