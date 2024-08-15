using WorldGenerator.Traits;

namespace WorldGenerator.AI;

public enum GoalState { New, Running, Completed, Failed, Cancelled }

public abstract class Goal : IGoal
{
    public IEntity? Owner { get; set; }

    public GoalState State { get; private set; }

    public IGoal? InterruptedWith { get; private set; }

    private IEnumerator<IGoalOrIntent?> _taskEnumerator = default!;

    public void Start()
    {
        _taskEnumerator = Execute().GetEnumerator();
    }

    protected virtual void OnCancel()
    {
        State = GoalState.Cancelled;
    }

    public void Cancel()
    {
        if (State is GoalState.Cancelled)
            return;

        OnCancel();
    }

    public void Tick()
    {
        if (State is GoalState.Completed or GoalState.Failed)
            return;

        if (Owner == null)
            throw new Exception($"Attempted to run {nameof(Goal)} without the {nameof(Owner)} set");

        if (State is GoalState.New)
            State = GoalState.Running;

        if (InterruptedWith != null)
        {
            InterruptedWith.Tick();

            if (InterruptedWith.State is GoalState.Completed)
            {
                InterruptedWith = null;
            }
            else if (InterruptedWith.State is GoalState.Failed)
            {
                State = GoalState.Failed;
                return;
            }
            else
            {
                return;
            }
        }

        if (!_taskEnumerator.MoveNext())
        {
            if (State == GoalState.Running)
                State = GoalState.Completed;

            return;
        }

        IGoalOrIntent? cur = _taskEnumerator.Current;
        if (cur is Goal goal)
        {
            InterruptedWith = goal;
            goal.Owner = Owner;
            goal.Start();
        }
        else if (cur is Intent intent)
        {
            IGoal? resolvedGoal = Owner.GetTrait<AITrait>().ResolveIntent(intent);
            if (resolvedGoal == null)
            {
                FailGoal();
                return;
            }
            InterruptedWith = resolvedGoal;
            resolvedGoal.Owner = Owner;
            resolvedGoal.Start();
        }
    }

    protected void FailGoal()
    {
        State = GoalState.Failed;
    }

    public abstract IEnumerable<IGoalOrIntent?> Execute();
}
