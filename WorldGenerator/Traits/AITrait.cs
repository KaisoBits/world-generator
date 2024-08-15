using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class AITrait : Trait<NullTraitData>
{
    private GoalTrait _goalTrait = default!;

    private readonly List<IDecision> _decisions = [];
    private IDecision? _currentDecision = null;

    private readonly DecisionFactory _decisionFactory;

    public AITrait(DecisionFactory decisionFactory)
    {
        _decisionFactory = decisionFactory;
    }

    protected override void OnGain()
    {
        _goalTrait = RequireTrait<GoalTrait>();
    }

    public override void Tick()
    {
        IDecision? bestDecision = MakeDecision();
        if (bestDecision != _currentDecision)
        {
            _currentDecision?.OnChosen();
            _goalTrait.AssignWork(bestDecision?.GetWork());
            _currentDecision = bestDecision;
        }

        if (!_goalTrait.IsBusy)
        {
            _currentDecision?.OnEnd();
            _currentDecision = null;
        }
    }

    private IDecision? MakeDecision()
    {
        IDecision? bestDecision = _decisions
            .Where(d => d.CanExecute())
            .MaxBy(d => d.GetPriority());

        return bestDecision;
    }

    public T RegisterDecision<T>() where T : IDecision
    {
        T decision = _decisionFactory.CreateDecision<T>();
        _decisions.Add(decision);
        return decision;
    }

    public void DeregisterDecision<T>() where T : IDecision
    {
        _decisions.RemoveAll(d => d is T);
    }
}

public enum DecisionPriority
{
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