namespace WorldGenerator.AI;

public enum SchedulerState { New, Running, Completed, Failed, Cancelling, Cancelled }

public abstract class Scheduler : IScheduler
{
    private readonly Dictionary<string, object?> _memory = [];

    public IEntity? Owner { get; set; }

    public SchedulerState State { get; private set; }

    public virtual SchedulerPriority Priority => SchedulerPriority.Default;

    public int CurrentTaskIndex { get; private set; } = -1;

    private IEnumerator<ISchedulerTask> _taskEnumerator = default!;

    public void Start()
    {
        _taskEnumerator = GetTasks().GetEnumerator();
    }

    public bool HasMemory(string memoryName)
    {
        return _memory.ContainsKey(memoryName);
    }

    public object? Recall(string memoryName)
    {
        return _memory.GetValueOrDefault(memoryName);
    }

    public void Remember(string memoryName)
    {
        Remember(memoryName, null);
    }

    public void Remember(string memoryName, object? memoryData)
    {
        _memory[memoryName] = memoryData;
    }

    public bool Forget(string memoryName)
    {
        return _memory.Remove(memoryName);
    }

    protected virtual void OnCancel()
    {
        State = SchedulerState.Cancelled;
    }

    public void Cancel() 
    {
        if (State is SchedulerState.Cancelled or SchedulerState.Cancelling)
            return;

        State = SchedulerState.Cancelling;
        OnCancel();
    }

    public void Tick()
    {
        if (State == SchedulerState.Completed)
            return;

        if (State == SchedulerState.New)
        {
            State = SchedulerState.Running;
            if (!_taskEnumerator.MoveNext())
            {
                State = SchedulerState.Completed;
                return;
            }
            CurrentTaskIndex++;
        }

        SchedulerTaskResult result = _taskEnumerator.Current.Tick();
        if (result == SchedulerTaskResult.Completed)
        {
            if (!_taskEnumerator.MoveNext())
                State = SchedulerState.Completed;

            CurrentTaskIndex++;
        }
        if (result == SchedulerTaskResult.Failed)
            State = SchedulerState.Failed;
    }

    public abstract IEnumerable<ISchedulerTask> GetTasks();
}
