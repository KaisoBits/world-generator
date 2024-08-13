namespace WorldGenerator;

public interface ITileView
{
    bool IsOccupied { get; }
    bool HasWall { get; }
    Vector Position { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
