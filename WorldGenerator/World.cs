using System.Diagnostics.CodeAnalysis;

namespace WorldGenerator;

public class World
{
    public bool Paused { get; set; }

    public int CurrentTick { get; private set; }

    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    public Vector Size => new(Width, Height, Depth);

    private readonly Tile[][] _tiles;
    public int Count => _tiles.Length;

    public IReadOnlyCollection<IEntity> Entities => _entities;
    private readonly List<IEntity> _entities = [];

    private readonly List<(IEntity Entity, Vector MoveTo)> _moveSchedule = [];

    private World(int x, int y, int z)
    {
        Width = x;
        Height = y;
        Depth = z;

        int len = x * y;

        _tiles = Enumerable.Range(0, z)
            .Select(zIndex =>
                Enumerable.Range(0, len)
                .Select(i =>
                {
                    bool hasWall = (zIndex == 0 ? false : Random.Shared.Next(0, 100) < 50);
                    return new Tile(i % Width, i / Width, zIndex)
                    {
                        HasFloor = true,
                        HasWall = hasWall
                    };
                })
                .ToArray())
            .ToArray();

        foreach (var tile in _tiles[0].Take(256 * 2))
        {
            tile.HasWall = true;
        }
    }

    public void Tick()
    {
        if (Paused)
            return;

        foreach (IEntity entity in _entities)
            entity.Think();

        _moveSchedule.Clear();

        CurrentTick++;
    }

    public ITileView this[int x, int y, int z] => GetTileAt(x, y, z);

    public ITileView this[Vector position] => GetTileAt(position);

    public static World CreateWorld(int width, int height, int depth)
    {
        return new(width, height, depth);
    }

    public void SpawnEntity(Entity entity, Vector position)
    {
        GetTileAt(position).AddEntity(entity);
        _entities.Add(entity);

        entity.Position = position;

        entity.OnSpawn();
    }

    public void RemoveWallAt(Vector position) => RemoveWallAt(position.X, position.Y, position.Z);
    public void RemoveWallAt(int x, int y, int z)
    {
        GetTileAt(x, y, z).HasWall = false;
    }

    public void MoveEntity(IEntity entity, Vector targetPosition)
    {
        if (entity.Position == targetPosition)
            return;

        GetTileAt(entity.Position).RemoveEntity(entity);
        GetTileAt(targetPosition).AddEntity(entity);

        ((Entity)entity).Position = targetPosition;
    }

    public IEnumerable<ITileView> GetTilesAtLevel(int zLevel)
    {
        return _tiles[zLevel];
    }

    public bool IsWithinBounds(Vector position)
    {
        return position.X >= 0 && position.Y >= 0 && position.Z >= 0 &&
            position.X < Width && position.Y < Height && position.Z < Depth;
    }

    public bool TryGetTile(int x, int y, int z, [NotNullWhen(true)] out ITileView? tile)
    {
        return TryGetTile(new Vector(x, y, z), out tile);
    }

    public bool TryGetTile(Vector position, [NotNullWhen(true)] out ITileView? tile)
    {
        if (!IsWithinBounds(position))
        {
            tile = null;
            return false;
        }

        tile = GetTileAt(position);

        return true;
    }

    private Tile GetTileAt(Vector position)
    {
        return GetTileAt(position.X, position.Y, position.Z);
    }

    private Tile GetTileAt(int x, int y, int z)
    {
        return _tiles[z][y * Width + x];
    }
}
