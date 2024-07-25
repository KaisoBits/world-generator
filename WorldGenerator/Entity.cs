using WorldGenerator.AI;


namespace WorldGenerator;

public abstract class Entity : IEntity
{
    private readonly HashSet<Condition> _conditions = [];
    private readonly Dictionary<State, string> _states = [];

    private readonly List<IBehaviour> _behaviours = [];

    public IScheduler? CurrentScheduler { get; private set; }

    public Position Position { get; internal set; }

    public ITileView CurrentTile => World.Instance[Position];

    public abstract Layer Layer { get; }

    public abstract void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);

    public virtual void Think()
    {
        CurrentScheduler?.Tick();

        if (CurrentScheduler == null)
        {
            foreach (var behaviour in _behaviours)
            {
                IScheduler? newScheduler = behaviour.DetermineScheduler(this);
                if (newScheduler != null)
                {
                    CurrentScheduler = newScheduler;
                    CurrentScheduler.Owner = this;
                    CurrentScheduler.Start();
                    CurrentScheduler.Tick();
                }
            }
        }

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

    protected void AddBehaviour(IBehaviour behaviour)
    {
        _behaviours.Add(behaviour);
    }
}
