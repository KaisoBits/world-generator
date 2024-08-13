using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class CanWalkTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _aiTrait.RegisterIntentResolver(ctx =>
        {
            if (ctx.Intent is GoToPositionIntent goToPosition)
            {
                float cost = (Owner.Position - goToPosition.TargetPosition).SimpleLen();
                ctx.AddGoalProposal(new GoalProposal(cost, (factory) =>
                {
                    return factory.CreateGoal<WalkToPositionGoal>().WithData(goToPosition.TargetPosition);
                }));
            }
        });
    }

    public override void OnLose()
    {

    }
}
