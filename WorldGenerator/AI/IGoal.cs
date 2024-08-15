namespace WorldGenerator.AI;

public interface IGoal : IWork
{
    IEntity? Owner { get; set; }
    GoalState State { get; }
    IGoal? InterruptedWith { get; }

    void Start();

    void Tick();

    void Cancel();
}
