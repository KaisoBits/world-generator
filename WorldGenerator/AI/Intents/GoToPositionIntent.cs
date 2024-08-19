namespace WorldGenerator.AI;

public class GoToPositionIntent : IIntent
{
    public Vector TargetPosition { get; }

    public GoToPositionIntent(Vector targetPosition)
    {
        TargetPosition = targetPosition;
    }
}
