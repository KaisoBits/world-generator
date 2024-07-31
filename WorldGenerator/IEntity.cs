using System.Diagnostics.CodeAnalysis;
using WorldGenerator.AI;
using WorldGenerator.EntityExtensions;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IEntity
{
    ISet<ICondition> Conditions { get; }
    IReadOnlyCollection<IState> States { get; }
    IReadOnlyCollection<ITrait> Traits { get; }

    IScheduler? CurrentScheduler { get; }

    Layer Layer { get; }
    public Vector Position { get; }

    ITileView CurrentTile { get; }

    bool IsSpawned { get; }

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

    void EnableExtension<T>() where T : IEntityExtension;
    T GetExtension<T>() where T : IEntityExtension;
    bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IEntityExtension;
}
