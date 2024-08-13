namespace WorldGenerator.AI;

public class MineBlockGoal : Goal
{
    private readonly World _world;

    public ITileView TargetTile { get; set; } = default!;

    public MineBlockGoal(World world)
    {
        _world = world;
    }

    public MineBlockGoal WithData(ITileView targetTile)
    {
        TargetTile = targetTile;
        return this;
    }

    public override IEnumerable<IGoalOrIntent?> Execute()
    {
        if (Owner == null)
        {
            FailGoal();
            yield break;
        }

        yield return new GoToPositionIntent(TargetTile.Position);

        if (!TargetTile.HasWall)
            yield break;

        int counter = 20;

        while (counter > 0)
        {
            counter--;
            yield return null;
        }

        _world.RemoveWallAt(TargetTile.Position);
    }
}
