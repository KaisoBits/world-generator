﻿using System.Collections;

namespace WorldGenerator;

public class World : IReadOnlyCollection<ITileView>
{
    public int CurrentTick { get; private set; }

    public static World Instance { get; private set; } = default!;

    public int Width { get; }
    public int Height { get; }

    private readonly Tile[] _tiles;
    public int Count => _tiles.Length;

    private List<(IEntity entity, Position moveTo)> _moveSchedule = [];

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

        foreach (var (ent, pos) in _moveSchedule)
            MoveEntity(ent, pos);
        _moveSchedule.Clear();


        CurrentTick++;
    }

    public ITileView this[int x, int y]
    {
        get => GetTileAt(x, y);
    }

    public ITileView this[Position position]
    {
        get => GetTileAt(position);
    }

    public static void CreateWorld(int width, int height)
    {
        World world = new(width, height);

        Instance = world;
    }

    public void SpawnEntity(Entity entity, Position position)
    {
        GetTileAt(position).AddEntity(entity);
        entity.Position = position;

        entity.OnSpawn();
    }

    public void ScheduleMoveEntity(IEntity entity, Position targetPosition)
    {
        _moveSchedule.Add((entity, targetPosition));
    }

    private void MoveEntity(IEntity entity, Position targetPosition)
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

    private Tile GetTileAt(Position position)
    {
        return GetTileAt(position.X, position.Y);
    }

    private Tile GetTileAt(int x, int y)
    {
        return _tiles[y * Width + x];
    }
}
