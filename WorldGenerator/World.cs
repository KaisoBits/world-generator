using System.Collections;

namespace WorldGenerator;

public class World : IReadOnlyCollection<ITileView>
{
    public bool Paused { get; set; }

    public int CurrentTick { get; private set; }

    public int Width { get; }
    public int Height { get; }

    private readonly Tile[] _tiles;
    public int Count => _tiles.Length;

    private readonly List<(IEntity Entity, Vector MoveTo)> _moveSchedule = [];

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
        if (Paused)
            return;

        foreach (var tile in _tiles)
            foreach (var contents in tile.Contents)
                contents.GatherConditions();

        foreach (var tile in _tiles)
            foreach (var contents in tile.Contents)
                contents.Think();

        foreach (var (ent, pos) in _moveSchedule)
            MoveEntity(ent, pos);

        _moveSchedule.Clear();

        CurrentTick++;
    }

    public ITileView this[int x, int y]
    {
        get => GetTileAt(x, y);
    }

    public ITileView this[Vector position]
    {
        get => GetTileAt(position);
    }

    public static World CreateWorld(int width, int height)
    {
        return new(width, height);
    }

    public void SpawnEntity(Entity entity, Vector position)
    {
        GetTileAt(position).AddEntity(entity);
        entity.Position = position;

        entity.OnSpawn();
    }

    public void ScheduleMoveEntity(IEntity entity, Vector targetPosition)
    {
        _moveSchedule.Add((entity, targetPosition));
    }

    private void MoveEntity(IEntity entity, Vector targetPosition)
    {
        if (entity.Position == targetPosition)
            return;

        GetTileAt(entity.Position).RemoveEntity(entity);
        GetTileAt(targetPosition).AddEntity(entity);

        ((Entity)entity).Position = targetPosition;
    }

    public IEnumerator<ITileView> GetEnumerator()
    {
        return _tiles.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Tile GetTileAt(Vector position)
    {
        return GetTileAt(position.X, position.Y);
    }

    private Tile GetTileAt(int x, int y)
    {
        return _tiles[y * Width + x];
    }
}
