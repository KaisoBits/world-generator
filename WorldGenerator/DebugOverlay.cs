namespace WorldGenerator;

public readonly record struct Highlight(Vector Position, int UntilTick);

public class DebugOverlay
{
    public DebugOverlay(World world)
    {
        _world = world;
    }

    public IReadOnlyCollection<Highlight> TileHighlights => _highlights;
    private readonly List<Highlight> _highlights = [];
    private readonly World _world;

    public void Tick()
    {
        _highlights.RemoveAll(h => _world.CurrentTick > h.UntilTick);
    }

    public void Clear()
    {
        _highlights.Clear();
    }

    public void HighlightTileAt(Vector position, int untilTick)
    {
        _highlights.RemoveAll(h => h.Position == position);

        _highlights.Add(new Highlight(position, untilTick));
    }
}
