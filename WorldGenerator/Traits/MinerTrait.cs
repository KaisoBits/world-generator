using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class MinerTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _aiTrait.RegisterIntentResolver(ctx =>
        {
            if (ctx.Intent is MineBlockIntent mineBlockIntent)
            {
                ctx.AddGoalProposal(new GoalProposal(10, (factory) =>
                {
                    return factory.CreateGoal<MineBlockGoal>().WithData(mineBlockIntent.TargetTile);
                }));
            }
        });
    }

    public override void OnLose()
    {

    }
}
