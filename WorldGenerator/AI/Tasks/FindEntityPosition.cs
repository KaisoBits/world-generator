namespace WorldGenerator.AI;

public class FindEntityPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly Layer _layer;
    private readonly string _positionMemory;

    public FindEntityPosition(IScheduler parent, Layer layer, string positionMemory)
    {
        _scheduler = parent;
        _layer = layer;
        _positionMemory = positionMemory;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Vector pos = _scheduler.Owner.Position;

        IEnumerable<ITileView> tiles = World.Instance
            .Where(t => t.Contents.Any(e => e.Layer == _layer));
        ITileView? closestTile = tiles.MinBy(t => Math.Abs(t.Position.X - pos.X) + Math.Abs(t.Position.Y - pos.Y));

        if (closestTile == null)
            return SchedulerTaskResult.Failed;

        _scheduler.Remember(_positionMemory, closestTile.Position);

        return SchedulerTaskResult.Completed;
    }
}
