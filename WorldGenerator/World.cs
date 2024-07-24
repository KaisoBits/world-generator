using System.Collections;

namespace WorldGenerator;

public class World : IReadOnlyCollection<ITileView>
{
    public static World Instance { get; private set; } = default!;

    public int Width { get; }
    public int Height { get; }

    private readonly Tile[] _tiles;
    public int Count => _tiles.Length;

    private World(int x, int y)
    {
        Width = x;
        Height = y;

        int len = x * y;
        _tiles = Enumerable.Range(0, len)
            .Select(i => new Tile(i % Width, i / Width))
            .ToArray();
    }

    public void Tick()
    {
        foreach (var tile in _tiles)
            foreach (var contents in tile.Contents)
                contents.GatherConditions();

        foreach (var tile in _tiles)
            foreach (var contents in tile.Contents)
                contents.Think();
    }

    public ITileView this[int x, int y]
    {
        get => GetTileAt(x, y);
    }

    public static void CreateWorld(int width, int height)
    {
        World world = new(width, height);

        Instance = world;
    }

    public void SpawnEntity(Entity entity, int x, int y)
    {
        GetTileAt(x, y).AddEntity(entity);
        entity.X = x;
        entity.Y = y;

        entity.OnSpawn();
    }

    public void MoveEntity(Entity entity, int x, int y)
    {
        if (x == entity.X && y == entity.Y)
            return;

        GetTileAt(entity.X, entity.Y).RemoveEntity(entity);
        GetTileAt(x, y).AddEntity(entity);
    }

    public IEnumerator<ITileView> GetEnumerator()
    {
        return _tiles.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Tile GetTileAt(int x, int y)
    {
        return _tiles[y * Width + x];
    }
}
