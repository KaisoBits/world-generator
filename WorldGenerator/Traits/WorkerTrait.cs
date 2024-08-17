using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class WorkerTrait : Trait<NullTraitData>
{
    private readonly JobOrderManager _workOrderManager;
    private readonly DecisionFactory _decisionFactory;

    private DoAssignedJobDecision? _decision;

    public IWork? AssignedWork { get; private set; }

    public WorkerTrait(JobOrderManager workOrderManager, DecisionFactory decisionFactory)
    {
        _workOrderManager = workOrderManager;
        _decisionFactory = decisionFactory;
    }

    protected override void OnGain()
    {
        _decision = _decisionFactory.CreateDecision<DoAssignedJobDecision>().WithData(this);
        Owner.AddToList<IDecision>(_decision);

        _workOrderManager.ReportForDuty(this);
    }

    public override void OnLose()
    {
        if (_decision != null)
            Owner.AddToList<IDecision>(_decision);

        _workOrderManager.RevertReportForDuty(this);
    }

    public bool AssignWorkOrder(IWork? work)
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
        _parent.AssignWorkOrder(null);
    }
}