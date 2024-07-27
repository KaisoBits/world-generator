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
    private (IScheduler Scheduler, AssignSchedulerResult Result)? _pendingScheduler;

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

        if (CurrentScheduler != null)
        {
            if (CurrentScheduler.State is SchedulerState.New)
            {
                CurrentScheduler.Start();
            }

            CurrentScheduler.Tick();

            if (CurrentScheduler.State is SchedulerState.Completed or SchedulerState.Failed or SchedulerState.Cancelled)
            {
                CurrentScheduler = _pendingScheduler?.Scheduler;
                if (_pendingScheduler != null)
                {
                    // Inform any interested parties that assigning this scheduler succeeded
                    // after it spent some time in the pending state
                    _pendingScheduler.Value.Result.SignalSuccess();
                    _pendingScheduler = null;
                }
            }
        }
    }

    public virtual void GatherConditions()
    {
    }

    public virtual void OnSpawn() { }
    public virtual void OnDespawn() { }

    public IAssignSchedulerResult AssignScheduler(IScheduler newScheduler)
    {
        // There is no current scheduler - just assign the one we got asked for
        if (CurrentScheduler == null)
        {
            CurrentScheduler = newScheduler;
            CurrentScheduler.Owner = this;
            return AssignSchedulerResult.Success();
        }

        // There is a current scheduler and its priority is higher than the new one - don't assign it
        if (CurrentScheduler.Priority >= newScheduler.Priority)
        {
            return AssignSchedulerResult.Fail();
        }

        // The current scheduler's priority is lower than the new one but we've already got a
        // pending scheduler which has higher priority than the new scheduler
        if (_pendingScheduler != null && _pendingScheduler.Value.Scheduler.Priority >= newScheduler.Priority)
        {
            return AssignSchedulerResult.Fail();
        }

        // If we reached this far, we have higher priority than both the current and pending schedulers.
        // If a pending scheduler exists, we can just override it
        if (_pendingScheduler != null)
        {
            // Inform any interested parties that assigning this scheduler failed
            // after it spent some time in the pending state and scheduler with
            // more priority came along
            _pendingScheduler.Value.Result.SignalFailed();

            AssignSchedulerResult newResult = AssignSchedulerResult.Pending();
            _pendingScheduler = (newScheduler, newResult);
            newScheduler.Owner = this;
            return newResult;
        }

        // There wasn't a pending scheduler before, so we cancel the current scheduler to "let it know" there's
        // something pending that has more priority than it does and it's time to die
        CurrentScheduler.Cancel();
        if (CurrentScheduler.State is SchedulerState.Cancelled)
        {
            CurrentScheduler = newScheduler;
            CurrentScheduler.Owner = this;
            return AssignSchedulerResult.Success();
        }

        // It didn't cancel immediately - means it must do some cleanup action
        // Let's schedule our future scheduler to be set once the old one is done cancelling
        AssignSchedulerResult result = AssignSchedulerResult.Pending();
        _pendingScheduler = (newScheduler, result);
        newScheduler.Owner = this;
        return result;
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

    public void AddTrait(ITrait trait)
    {
        _traits.Add(trait);
        trait.OnGain(this);
    }
}
