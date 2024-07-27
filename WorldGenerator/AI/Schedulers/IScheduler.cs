using WorldGenerator.AI.Schedulers;

namespace WorldGenerator.AI;

public interface IScheduler
{
    IEntity? Owner { get; set; }
    SchedulerState State { get; }
    SchedulerPriority Priority { get; }

    void Start();

    void Tick();

    bool HasMemory(string memoryName);

    void Remember(string memoryName);

    void Remember(string memoryName, object? memoryData);

    bool Forget(string memoryName);

    void Cancel();
}
