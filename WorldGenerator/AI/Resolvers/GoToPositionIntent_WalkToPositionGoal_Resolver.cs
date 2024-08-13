using WorldGenerator.Traits;

namespace WorldGenerator.AI;

public class GoToPositionIntent_WalkToPositionGoal_Resolver : IIntentResolver
{
    public void Resolve(IIntentResolverContext ctx)
    {
        if (ctx.Intent is GoToPositionIntent goToPosition)
        {
            float cost = (ctx.AITrait.Owner.Position - goToPosition.TargetPosition).SimpleLen();
            ctx.AddGoalProposal(new GoalProposal(cost, (factory) =>
            {
                return factory.CreateGoal<WalkToPositionGoal>().WithData(goToPosition.TargetPosition);
            }));
        }
    }
}
