namespace WorldGenerator;

public class Tile : ITileView
{
    public IReadOnlyList<IEntity> Contents => _contents;

    public bool IsOccupied => HasWall || _contents is not [];

    public bool HasFloor { get; set; }
    public bool HasWall { get; set; }

    public Vector Position { get; }

    private readonly List<IEntity> _contents = [];

    public Tile(int x, int y, int z)
    {
        Position = new Vector(x, y, z);
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
