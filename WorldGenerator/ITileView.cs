namespace WorldGenerator;

public interface ITileView
{
    bool IsOccupied { get; }
    bool HasWall { get; }
    bool HasFloor { get; }
    Vector Position { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
