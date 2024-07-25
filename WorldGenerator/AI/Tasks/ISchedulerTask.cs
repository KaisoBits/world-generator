namespace WorldGenerator.AI;

public enum SchedulerTaskResult { Continue, Completed, Failed };

public interface ISchedulerTask
{
    SchedulerTaskResult Tick();
}
