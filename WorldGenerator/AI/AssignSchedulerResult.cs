namespace WorldGenerator.AI;

public enum AssignSchedulerResultType { Success, Pending, Failed }

public class AssignSchedulerResult : IAssignSchedulerResult
{
    private readonly static AssignSchedulerResult _success = new() { Result = AssignSchedulerResultType.Success };
    private readonly static AssignSchedulerResult _fail = new() { Result = AssignSchedulerResultType.Success };
    private readonly static AssignSchedulerResult _pending = new() { Result = AssignSchedulerResultType.Success };

    public AssignSchedulerResultType Result { get; private set; }

    public static AssignSchedulerResult Success() => _success;
    public static AssignSchedulerResult Fail() => _fail;
    public static AssignSchedulerResult Pending() => _pending;

    public void SignalSuccess()
    {
        if (Result != AssignSchedulerResultType.Pending)
            throw new Exception($"Attempted to set {nameof(AssignSchedulerResult)} to " +
                $"{nameof(AssignSchedulerResultType.Success)} but it wasn't {nameof(AssignSchedulerResultType.Pending)} before");

        Result = AssignSchedulerResultType.Success;
    }

    public void SignalFailed()
    {
        if (Result != AssignSchedulerResultType.Pending)
            throw new Exception($"Attempted to set {nameof(AssignSchedulerResult)} to " +
                $"{nameof(AssignSchedulerResultType.Failed)} but it wasn't {nameof(AssignSchedulerResultType.Pending)} before");

        Result = AssignSchedulerResultType.Failed;
    }

}

public interface IAssignSchedulerResult
{
    AssignSchedulerResultType Result { get; }
}