namespace WorldGenerator.AI;

public class GoToPositionIntent : Intent
{
    public Vector TargetPosition { get; }

    public GoToPositionIntent(Vector targetPosition)
    {
        TargetPosition = targetPosition;
    }
}
