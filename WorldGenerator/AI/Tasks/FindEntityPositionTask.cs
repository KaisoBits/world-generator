namespace WorldGenerator.AI;

public class FindEntityPositionTask : ISchedulerTask
{
    private readonly World _world;
    private readonly IScheduler _scheduler;
    private Layer _layer = default!;
    private string _positionMemory = default!;

    public FindEntityPositionTask(World world, IScheduler parent)
    {
        _world = world;
        _scheduler = parent;
    }

    public FindEntityPositionTask WithData(Layer layer, string positionMemory)
    {
        _layer = layer;
        _positionMemory = positionMemory;

        return this;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Vector pos = _scheduler.Owner.Position;

        IEnumerable<ITileView> tiles = _world
            .Where(t => t.Contents.Any(e => e.Layer == _layer));
        ITileView? closestTile = tiles.MinBy(t => Math.Abs(t.Position.X - pos.X) + Math.Abs(t.Position.Y - pos.Y));

        if (closestTile == null)
            return SchedulerTaskResult.Failed;

        _scheduler.Remember(_positionMemory, closestTile.Position);

        return SchedulerTaskResult.Completed;
    }
}
