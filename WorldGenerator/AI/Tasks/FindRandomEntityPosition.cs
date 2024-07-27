namespace WorldGenerator.AI.Tasks;

public class FindRandomEntityPosition : ISchedulerTask
{
    private readonly IScheduler _scheduler;
    private readonly Layer _layer;
    private readonly string _positionMemory;

    public FindRandomEntityPosition(IScheduler parent, Layer layer, string positionMemory)
    {
        _scheduler = parent;
        _layer = layer;
        _positionMemory = positionMemory;
    }

    public SchedulerTaskResult Tick()
    {
        if (_scheduler.Owner == null)
            return SchedulerTaskResult.Failed;

        Position pos = _scheduler.Owner.Position;

        ITileView[] tiles = World.Instance
            .Where(t => t.Contents.Any(e => e.Layer == _layer)).ToArray();

        if (tiles is [])
            return SchedulerTaskResult.Failed;

        ITileView target = Random.Shared.GetItems(tiles, 1)[0];

        _scheduler.Remember(_positionMemory, target.Position);

        return SchedulerTaskResult.Completed;
    }
}
