using WorldGenerator.AI;
using WorldGenerator.Traits;

namespace WorldGenerator;

public abstract class Entity : IEntity
{
    public IReadOnlyCollection<Condition> Conditions => _conditions;
    private readonly HashSet<Condition> _conditions = [];
    public IReadOnlyDictionary<State, string> States => _states;
    private readonly Dictionary<State, string> _states = [];

    public IReadOnlyCollection<ITrait> Traits => _traits;
    private readonly List<ITrait> _traits = [];

    public IScheduler? CurrentScheduler { get; private set; }

    public Position Position { get; internal set; }

    public ITileView CurrentTile => World.Instance[Position];

    public abstract Layer Layer { get; }

    public abstract void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);

    public virtual void Think()
    {
        foreach (ITrait trait in _traits)
        {
            trait.Tick();
        }

        CurrentScheduler?.Tick();

        if (CurrentScheduler != null)
        {
            if (CurrentScheduler is { State: SchedulerState.Completed } or { State: SchedulerState.Failed })
                CurrentScheduler = null;
        }
    }

    public virtual void GatherConditions()
    {
    }

    public virtual void OnSpawn() { }
    public virtual void OnDespawn() { }

    public void SetScheduler(IScheduler? scheduler)
    {
        if (CurrentScheduler?.State is SchedulerState.Running or SchedulerState.New)
            CurrentScheduler.OnCancel();

        CurrentScheduler = scheduler;
        if (scheduler != null)
        {
            scheduler.Owner = this;
            scheduler.Start();
        }
    }

    public bool InCondition(Condition condition)
    {
        return _conditions.Contains(condition);
    }

    public void SetCondition(Condition condition)
    {
        _conditions.Add(condition);
    }

    public void ClearCondition(Condition condition)
    {
        _conditions.Remove(condition);
    }

    public void SetState(State state, string value)
    {
        _states[state] = value;
    }

    public string? GetState(State state)
    {
        return _states.GetValueOrDefault(state);
    }

    public int GetStateInt(State state)
    {
        return int.Parse(_states[state]);
    }

    public float GetStateFloat(State state)
    {
        return float.Parse(_states[state]);
    }

    protected void AddTrait(ITrait trait)
    {
        _traits.Add(trait);
        trait.OnGain(this);
    }
}
