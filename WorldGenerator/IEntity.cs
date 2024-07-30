using WorldGenerator.AI;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IEntity
{
    ISet<ICondition> Conditions { get; }
    IReadOnlyCollection<IState> States { get; }

    IScheduler? CurrentScheduler { get; }

    Layer Layer { get; }
    public Vector Position { get; }

    ITileView CurrentTile { get; }

    void GatherConditions();
    void Think();

    void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);

    IAssignSchedulerResult AssignScheduler(IScheduler scheduler);

    bool InCondition<T>() where T : ICondition;
    void SetCondition<T>() where T : ICondition;
    bool ClearCondition<T>() where T : ICondition;

    void SetState(IState state);

    T? GetState<T>() where T : class, IState;

    void AddTrait(ITrait trait);
}
