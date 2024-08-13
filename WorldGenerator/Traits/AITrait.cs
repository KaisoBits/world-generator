using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class AITrait : Trait<NullTraitData>
{
    public (IGoal Goal, GoalPriority Priority)? CurrentGoal => _currentGoal;
    private (IGoal Goal, GoalPriority Priority)? _currentGoal;

    private readonly List<(IIntentResolver Resolver, int Count)> _intentResolvers = [];

    private readonly GoalFactory _goalFactory;
    private readonly IntentResolverFactory _intentResolverFactory;

    public AITrait(GoalFactory goalFactory, IntentResolverFactory intentResolverFactory)
    {
        _goalFactory = goalFactory;
        _intentResolverFactory = intentResolverFactory;
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

    public void RegisterIntentResolver<T>() where T : IIntentResolver
    {
        int index = _intentResolvers.FindIndex(r => r.Resolver is T);
        if (index == -1)
        {
            _intentResolvers.Add((_intentResolverFactory.CreateResolver<T>(), 1));
        }
        else
        {
            var (resolver, count) = _intentResolvers[index];
            _intentResolvers[index] = (resolver, count + 1);
        }
    }

    public void DeregisterIntentResolver<T>() where T : IIntentResolver
    {
        int index = _intentResolvers.FindIndex(r => r.Resolver is T);
        if (index == -1)
            throw new Exception($"Attempted to deregister resolver of type {typeof(T).Name} that wasn't registered");

        var (resolver, count) = _intentResolvers[index];
        if (count <= 1)
            _intentResolvers.RemoveAt(index);
        else
            _intentResolvers[index] = (resolver, count - 1);
    }

    public IGoal? ResolveIntent(Intent intent)
    {
        var ctx = new IntentResolverContext { Intent = intent, AITrait = this };

        foreach (IIntentResolver resolver in _intentResolvers.Select(r => r.Resolver))
            resolver.Resolve(ctx);

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
    AITrait AITrait { get; }
    Intent Intent { get; }
    void AddGoalProposal(GoalProposal proposal);
}

public class IntentResolverContext : IIntentResolverContext
{
    public IReadOnlyCollection<GoalProposal> PossibleGoals => _possibleGoals;
    private readonly List<GoalProposal> _possibleGoals = [];

    public required AITrait AITrait { get; init; }
    public required Intent Intent { get; init; }

    public void AddGoalProposal(GoalProposal proposal)
    {
        _possibleGoals.Add(proposal);
    }
}

public record class GoalProposal(float Cost, Func<GoalFactory, IGoal> GoalFactory);
