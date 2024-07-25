namespace WorldGenerator;

public class Tile : ITileView
{
    public IReadOnlyList<IEntity> Contents => _contents;

    public Position Position { get; }

    private readonly List<IEntity> _contents = [];

    public Tile(int x, int y)
    {
        Position = new Position(x, y);
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
