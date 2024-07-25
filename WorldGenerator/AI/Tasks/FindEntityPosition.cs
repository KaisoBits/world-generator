namespace WorldGenerator.AI;

public class FindEntityPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly Layer _layer;
    private readonly string _positionMemory;
    private readonly bool _getFurthest;

    public FindEntityPosition(IScheduler parent, Layer layer, string positionMemory, bool max)
    {
        _scheduler = parent;
        _layer = layer;
        _positionMemory = positionMemory;
        _getFurthest = max;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Position pos = _scheduler.Owner.Position;

        IEnumerable<ITileView> tiles = World.Instance
            .Where(t => t.Contents.Any(e => e.Layer == _layer));
        ITileView? closestTile = _getFurthest ?
            tiles.MaxBy(t => Math.Abs(t.Position.X - pos.X) + Math.Abs(t.Position.Y - pos.Y)) :
            tiles.MinBy(t => Math.Abs(t.Position.X - pos.X) + Math.Abs(t.Position.Y - pos.Y));

        if (closestTile == null)
            return SchedulerTaskResult.Failed;

        _scheduler.Remember(_positionMemory, closestTile.Position);

        return SchedulerTaskResult.Completed;
    }
}
