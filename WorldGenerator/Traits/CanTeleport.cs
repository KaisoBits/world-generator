using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class CanTeleport : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _aiTrait.RegisterIntentResolver(ctx =>
        {
            if (ctx.Intent is GoToPositionIntent goToPosition)
            {
                ctx.AddGoalProposal(new GoalProposal(5, (factory) =>
                {
                    return factory.CreateGoal<TeleportToPositionGoal>().WithData(goToPosition.TargetPosition);
                }));
            }
        });
    }

    public override void OnLose()
    {

    }
}
