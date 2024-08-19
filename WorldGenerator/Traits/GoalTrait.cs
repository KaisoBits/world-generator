using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class GoalTrait : Trait<NullTraitData>
{
    public bool IsBusy => CurrentGoal != null;
    public IGoal? CurrentGoal { get; private set; }

    private readonly GoalFactory _goalFactory;

    public GoalTrait(GoalFactory goalFactory)
    {
        _goalFactory = goalFactory;
    }

    public override void Tick()
    {
        if (CurrentGoal == null)
            return;

        IGoal goal = CurrentGoal;

        if (goal.State is GoalState.New)
        {
            goal.Start();
        }

        goal.Tick();

        if (goal.State is GoalState.Completed or GoalState.Failed or GoalState.Cancelled)
        {
            CurrentGoal = null;
        }
    }

    public IGoal? ResolveIntent(IIntent intent)
    {
        var ctx = new IntentResolverContext { Intent = intent, AITrait = this };

        IEnumerable<IIntentResolver> intentResolvers = Owner.GetList<IIntentResolver>();
        foreach (IIntentResolver resolver in intentResolvers)
            resolver.Resolve(ctx);

        GoalProposal? cheapestProposal = ctx.PossibleGoals.MinBy(pg => pg.Cost);

        if (cheapestProposal == null)
            return null;

        return cheapestProposal.GoalFactory(_goalFactory);
    }

    public IGoal? AssignWork(IWork? goalOrIntent)
    {
        IGoal? goal = null;
        if (goalOrIntent is IIntent intent)
            goal = ResolveIntent(intent);
        else if (goalOrIntent is IGoal g)
            goal = g;

        CurrentGoal?.Cancel();

        if (goal == null)
            return null;

        goal.Owner = Owner;
        CurrentGoal = goal;
        return goal;
    }
}

public interface IIntentResolverContext
{
    IEntity Owner { get; }
    GoalTrait AITrait { get; }
    IIntent Intent { get; }
    void AddGoalProposal(GoalProposal proposal);
}

public class IntentResolverContext : IIntentResolverContext
{
    public IReadOnlyCollection<GoalProposal> PossibleGoals => _possibleGoals;
    private readonly List<GoalProposal> _possibleGoals = [];

    public IEntity Owner => AITrait.Owner;
    public required GoalTrait AITrait { get; init; }
    public required IIntent Intent { get; init; }

    public void AddGoalProposal(GoalProposal proposal)
    {
        _possibleGoals.Add(proposal);
    }
}

public record class GoalProposal(float Cost, Func<GoalFactory, IGoal> GoalFactory);
