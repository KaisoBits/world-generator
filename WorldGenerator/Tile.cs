namespace WorldGenerator;

public class Tile
{
    public IReadOnlyList<IEntity> Contents => _contents;
    private readonly List<IEntity> _contents = [];

    public void AddEntity(IEntity entity)
    {
        _contents.Add(entity);
        _contents.Sort((a, b) => (int)b.Layer - (int)a.Layer);
    }
}
