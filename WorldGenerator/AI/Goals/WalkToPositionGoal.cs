using WorldGenerator.States;

namespace WorldGenerator.AI;

public class WalkToPositionGoal : Goal
{
    private readonly World _world;

    public Vector TargetPosition { get; set; }

    public WalkToPositionGoal(World world)
    {
        _world = world;
    }

    public WalkToPositionGoal WithData(Vector targetPosition)
    {
        TargetPosition = targetPosition;
        return this;
    }

    public override IEnumerable<IGoalOrIntent?> Execute()
    {
        if (Owner == null)
        {
            FailGoal();
            yield break;
        }

        while (true)
        {
            int counter = 20;

            while (counter > 0)
            {
                counter -= (Owner.GetState<SpeedState>()?.Speed ?? 10);
                yield return null;
            }

            Vector currentPos = Owner.Position;
            Vector offset = TargetPosition - currentPos;

            if (offset == Vector.Zero)
                break;

            Vector newPos;
            if (Math.Abs(offset.X) > Math.Abs(offset.Y))
            {
                newPos = currentPos + new Vector(offset.X / Math.Abs(offset.X), 0);
                _world.MoveEntity(Owner, newPos);
            }
            else
            {
                newPos = currentPos + new Vector(0, offset.Y / Math.Abs(offset.Y));
                _world.MoveEntity(Owner, newPos);
            }

            yield return null;
        }
    }
}
