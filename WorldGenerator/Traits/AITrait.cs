using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class AITrait : Trait<NullTraitData>
{
    public (IGoal Goal, GoalPriority Priority)? CurrentGoal => _currentGoal;
    private (IGoal Goal, GoalPriority Priority)? _currentGoal;

    private readonly List<Action<IntentResolverContext>> _intentResolvers = [];

    private readonly GoalFactory _goalFactory;


    public AITrait(GoalFactory goalFactory)
    {
        _goalFactory = goalFactory;
    }

    public override void Tick()
    {
        if (_currentGoal == null)
            return;

        var (goal, _) = _currentGoal.Value;

        if (goal.State is GoalState.New)
        {
            goal.Start();
        }

        goal.Tick();

        if (goal.State is GoalState.Completed or GoalState.Failed or GoalState.Cancelled)
        {
            _currentGoal = null;
        }
    }

    public void RegisterIntentResolver(Action<IIntentResolverContext> resolver)
    {
        _intentResolvers.Add(resolver);
    }

    public void DeregisterIntentResolver(Action<IIntentResolverContext> resolver)
    {
        _intentResolvers.Remove(resolver);
    }

    public IGoal? ResolveIntent(Intent intent)
    {
        var ctx = new IntentResolverContext { Intent = intent };

        foreach (Action<IntentResolverContext> resolver in _intentResolvers)
            resolver(ctx);

        GoalProposal? cheapestProposal = ctx.PossibleGoals.MinBy(pg => pg.Cost);

        if (cheapestProposal == null)
            return null;

        return cheapestProposal.GoalFactory(_goalFactory);
    }

    public IGoal? AssignWork(IGoalOrIntent? goalOrIntent)
    {
        return AssignWork(GoalPriority.Default, goalOrIntent);
    }

    public IGoal? AssignWork(GoalPriority priority, IGoalOrIntent? goalOrIntent)
    {
        IGoal? goal = null;
        if (goalOrIntent is Intent intent)
            goal = ResolveIntent(intent);
        else if (goalOrIntent is IGoal g)
            goal = g;

        if (goal == null)
            return null;

        if (_currentGoal == null || (int)_currentGoal.Value.Priority < (int)priority)
        {
            goal.Owner = Owner;
            _currentGoal = (goal, priority);
            return goal;
        }

        return null;
    }
}

public interface IIntentResolverContext
{
    Intent Intent { get; }
    void AddGoalProposal(GoalProposal proposal);
}

public class IntentResolverContext : IIntentResolverContext
{
    public IReadOnlyCollection<GoalProposal> PossibleGoals => _possibleGoals;
    private readonly List<GoalProposal> _possibleGoals = [];

    public required Intent Intent { get; init; }

    public void AddGoalProposal(GoalProposal proposal)
    {
        _possibleGoals.Add(proposal);
    }
}

public record class GoalProposal(float Cost, Func<GoalFactory, IGoal> GoalFactory);
