using System.Diagnostics.CodeAnalysis;
using WorldGenerator.RenderActors;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IEntity
{
    ISet<ICondition> Conditions { get; }
    IReadOnlyCollection<IState> States { get; }
    IReadOnlyCollection<ITrait> Traits { get; }

    Layer Layer { get; }

    IRenderActor? RenderActor { get; }

    public Vector Position { get; }

    ITileView CurrentTile { get; }

    bool IsSpawned { get; }

    void GatherConditions();
    void Think();

    void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);

    bool InCondition<T>() where T : ICondition;
    void SetCondition<T>() where T : ICondition;
    bool ClearCondition<T>() where T : ICondition;

    void SetState(IState state);

    T? GetState<T>() where T : class, IState;

    T GetOrAddTrait<T>() where T : ITrait;
    T AddTrait<T>() where T : ITrait;
    bool TryAddTrait<T>() where T : ITrait;
    bool HasTrait<T>() where T : ITrait;
    T GetTrait<T>() where T : ITrait;
    bool TryGetTrait<T>([NotNullWhen(true)] out T? trait) where T : ITrait;
}
