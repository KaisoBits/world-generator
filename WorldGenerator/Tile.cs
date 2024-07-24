namespace WorldGenerator;

public class Tile : ITileView
{
    public IReadOnlyList<IEntity> Contents => _contents;

    public int X { get; }
    public int Y { get; }

    private readonly List<IEntity> _contents = [];

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void AddEntity(IEntity entity)
    {
        _contents.Add(entity);
        _contents.Sort((a, b) => (int)b.Layer - (int)a.Layer);
    }

    public void RemoveEntity(IEntity entity)
    {
        _contents.Remove(entity);
    }
}
