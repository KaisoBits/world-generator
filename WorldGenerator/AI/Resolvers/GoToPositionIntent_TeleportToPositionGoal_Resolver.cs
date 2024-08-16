using WorldGenerator.Traits;

namespace WorldGenerator.AI;

public class GoToPositionIntent_TeleportToPositionGoal_Resolver : IIntentResolver
{
    public void Resolve(IIntentResolverContext ctx)
    {
        if (ctx.Intent is GoToPositionIntent goToPosition)
        {
            ctx.AddGoalProposal(new GoalProposal(5, (factory) =>
                factory.CreateGoal<TeleportToPositionGoal>().WithData(goToPosition.TargetPosition)));
        }
    }
}