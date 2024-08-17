using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class AITrait : Trait<NullTraitData>
{
    private GoalTrait _goalTrait = default!;

    private IDecision? _currentDecision = null;

    protected override void OnGain()
    {
        _goalTrait = RequireTrait<GoalTrait>();
    }

    public override void Tick()
    {
        IDecision? bestDecision = MakeDecision();
        if (bestDecision != _currentDecision && (_currentDecision == null || bestDecision == null || bestDecision.GetPriority() > _currentDecision.GetPriority()))
        {
            _currentDecision?.OnEnd();
            _goalTrait.AssignWork(bestDecision?.GetWork());
            bestDecision?.OnChosen();
            _currentDecision = bestDecision;
        }

        if (_currentDecision != null && !_goalTrait.IsBusy)
        {
            _currentDecision?.OnEnd();
            _currentDecision = null;
        }
    }

    private IDecision? MakeDecision()
    {
        IEnumerable<IDecision> decisions = Owner.GetList<IDecision>();

        IDecision? bestDecision = decisions
            .Where(d => d.CanExecute())
            .MaxBy(d => d.GetPriority());

        return bestDecision;
    }
}

public enum DecisionPriority
{
    Idle,
    Low,
    Default,
    Emergency,
    Panic,
}

public interface IDecision
{
    void OnChosen();
    void OnEnd();
    bool CanExecute();
    DecisionPriority GetPriority();
    IWork GetWork();
}