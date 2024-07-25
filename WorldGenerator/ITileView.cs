namespace WorldGenerator;

public interface ITileView
{
    Position Position { get; }
    IReadOnlyList<IEntity> Contents { get; }
}
