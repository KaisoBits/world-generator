using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class WorkerTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    private readonly JobOrderManager _workOrderManager;

    public IWork? AssignedWork { get; private set; }

    public WorkerTrait(JobOrderManager workOrderManager)
    {
        _workOrderManager = workOrderManager;
    }

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();
        _aiTrait.RegisterDecision<DoAssignedJobDecision>().WithData(this);

        _workOrderManager.ReportForDuty(this);
    }

    public override void OnLose()
    {
        _aiTrait.DeregisterDecision<DoAssignedJobDecision>();

        _workOrderManager.RevertReportForDuty(this);
    }

    public bool AssignWorkOrder(IWork work)
    {
        AssignedWork = work;

        return true;
    }
}

public class DoAssignedJobDecision : IDecision
{
    private readonly JobOrderManager _workOrderManager;

    private WorkerTrait _parent = default!;

    public DoAssignedJobDecision(JobOrderManager workOrderManager)
    {
        _workOrderManager = workOrderManager;
    }

    public DoAssignedJobDecision WithData(WorkerTrait parent)
    {
        _parent = parent;
        return this;
    }

    public bool CanExecute() => _parent.AssignedWork != null;

    public DecisionPriority GetPriority() => DecisionPriority.Default;

    public IWork GetWork() => _parent.AssignedWork ??
        throw new Exception($"Attempted to create work when ${nameof(CanExecute)} should've returned false");

    public void OnChosen()
    {
        _workOrderManager.RevertReportForDuty(_parent);
    }

    public void OnEnd()
    {
        _workOrderManager.ReportForDuty(_parent);
    }
}