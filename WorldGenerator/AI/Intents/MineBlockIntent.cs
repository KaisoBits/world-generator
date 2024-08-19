namespace WorldGenerator.AI;

public class MineBlockIntent : IIntent
{
    public ITileView TargetTile { get; }

    public MineBlockIntent(ITileView tileView)
    {
        TargetTile = tileView;
    }
}
