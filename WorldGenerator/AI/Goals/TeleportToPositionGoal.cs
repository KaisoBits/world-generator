namespace WorldGenerator.AI;

public class TeleportToPositionGoal : Goal
{
    private readonly World _world;

    public Vector TargetPosition { get; set; }

    public TeleportToPositionGoal(World world)
    {
        _world = world;
    }

    public TeleportToPositionGoal WithData(Vector targetPosition)
    {
        TargetPosition = targetPosition;
        return this;
    }

    public override IEnumerable<IWork?> Execute()
    {
        if (Owner == null)
        {
            FailGoal();
            yield break;
        }

        int counter = 5;

        while (counter > 0)
        {
            counter -= 1;
            yield return null;
        }

        _world.MoveEntity(Owner, TargetPosition);

    }
}