namespace WorldGenerator.AI;

public class MineBlockIntent : Intent
{
    public ITileView TargetTile { get; }

    public MineBlockIntent(ITileView tileView)
    {
        TargetTile = tileView;
    }
}
