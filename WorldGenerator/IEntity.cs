using WorldGenerator.AI;

namespace WorldGenerator;

public interface IEntity
{
    IReadOnlyCollection<Condition> Conditions { get; }
    IReadOnlyDictionary<State, string> States { get; }

    IScheduler? CurrentScheduler { get; }

    Layer Layer { get; }
    public Position Position { get; }

    ITileView CurrentTile { get; }

    void GatherConditions();
    void Think();

    void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);

    bool InCondition(Condition condition);
    void SetCondition(Condition condition);
    void ClearCondition(Condition condition);

    void SetState(State state, string value);

    string? GetState(State state);
    int GetStateInt(State state);
    float GetStateFloat(State state);
}
