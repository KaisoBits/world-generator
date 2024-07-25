namespace WorldGenerator.AI;

public enum SchedulerState { New, Running, Completed, Failed }

public abstract class Scheduler : IScheduler
{
    private readonly Dictionary<string, object?> _memory = [];

    public IEntity? Owner { get; set; }

    public SchedulerState State { get; private set; }

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

    public void Tick()
    {
        if (State == SchedulerState.Completed)
            return;

        if (State == SchedulerState.New)
        {
            State = SchedulerState.Running;
            _taskEnumerator.MoveNext();
        }

        if (_taskEnumerator.Current == null)
        {
            State = SchedulerState.Completed;
            return;
        }

        SchedulerTaskResult result = _taskEnumerator.Current.Tick();
        if (result == SchedulerTaskResult.Completed)
            _taskEnumerator.MoveNext();
        if (result == SchedulerTaskResult.Failed)
            State = SchedulerState.Failed;
    }

    public abstract IEnumerable<ISchedulerTask> GetTasks();
}
