using WorldGenerator.Traits;

namespace WorldGenerator.AI;

public class MineBlockIntent_MineBlockGoal_Resolver : IIntentResolver
{
    public void Resolve(IIntentResolverContext ctx)
    {
        if (ctx.Intent is MineBlockIntent mineBlockIntent)
        {
            ctx.AddGoalProposal(new GoalProposal(10, (factory) =>
            {
                return factory.CreateGoal<MineBlockGoal>().WithData(mineBlockIntent.TargetTile);
            }));
        }
    }
}
