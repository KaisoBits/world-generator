namespace WorldGenerator.Tasks;

public enum SchedulerTaskResult { Continue, Completed, Failed };

public interface ISchedulerTask
{
    SchedulerTaskResult Tick();
}
