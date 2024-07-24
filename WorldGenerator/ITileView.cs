namespace WorldGenerator;

public interface ITileView
{
    int X { get; }
    int Y { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
