using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class WorkerTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    private readonly WorkOrderManager _workOrderManager;

    private IGoal? _workGoal;

    public WorkerTrait(WorkOrderManager workOrderManager)
    {
        _workOrderManager = workOrderManager;
    }

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _workOrderManager.ReportForDuty(this);
    }

    public override void OnLose()
    {
        _workOrderManager.RevertReportForDuty(this);
    }

    public bool AssignWorkOrder(IGoalOrIntent goal)
    {
        IGoal? result = _aiTrait.AssignWork(goal);

        if (result == null)
            return false;

        _workGoal = result;

        return true;
    }

    public override void Tick()
    {
        if (_workGoal?.State is GoalState.Failed or GoalState.Completed)
        {
            _workGoal = null;
            _workOrderManager.ReportForDuty(this);
        }
    }
}
