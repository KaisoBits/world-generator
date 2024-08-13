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

    public IReadOnlyCollection<IEntity> Entities => _entities;
    private readonly List<IEntity> _entities = [];

    public IReadOnlyCollection<IEntity> SystemEntities => _systemEntities;
    private readonly List<IEntity> _systemEntities = [];

    private readonly List<(IEntity Entity, Vector MoveTo)> _moveSchedule = [];

    private World(int x, int y)
    {
        Width = x;
        Height = y;

        int len = x * y;

        _tiles = Enumerable.Range(0, len)
            .Select(i => new Tile(i % Width, i / Width))
            .ToArray();

        foreach (var tile in _tiles.Take(40))
        {
            tile.HasWall = true;
        }
    }

    public void Tick()
    {
        if (Paused)
            return;

        foreach (IEntity entity in _entities)
            entity.GatherConditions();

        foreach (IEntity entity in _systemEntities)
            entity.GatherConditions();

        foreach (IEntity entity in _entities)
            entity.Think();

        foreach (IEntity entity in _systemEntities)
            entity.Think();

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
        _entities.Add(entity);

        entity.Position = position;

        entity.OnSpawn();
    }

    public void ScheduleMoveEntity(IEntity entity, Vector targetPosition)
    {
        _moveSchedule.Add((entity, targetPosition));
    }

    public void RemoveWallAt(Vector position) => RemoveWallAt(position.X, position.Y);
    public void RemoveWallAt(int x, int y)
    {
        GetTileAt(x, y).HasWall = false;
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
