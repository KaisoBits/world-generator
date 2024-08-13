namespace WorldGenerator.AI;

public interface IGoal : IGoalOrIntent
{
    IEntity? Owner { get; set; }
    GoalState State { get; }
    IGoal? InterruptedWith { get; }

    void Start();

    void Tick();

    void Cancel();
}
