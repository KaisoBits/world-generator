namespace WorldGenerator;

public interface ITileView
{
    Vector Position { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
