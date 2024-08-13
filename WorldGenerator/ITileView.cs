namespace WorldGenerator;

public interface ITileView
{
    bool IsOccupied { get; }
    bool HasWall { get; set; }
    Vector Position { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
