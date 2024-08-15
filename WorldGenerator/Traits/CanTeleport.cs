using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class CanTeleport : Trait<NullTraitData>
{
    private GoalTrait _goalTrait = default!;

    protected override void OnGain()
    {
        _goalTrait = RequireTrait<GoalTrait>();

        _goalTrait.RegisterIntentResolver<GoToPositionIntent_TeleportToPositionGoal_Resolver>();
    }

    public override void OnLose()
    {
        _goalTrait.DeregisterIntentResolver<GoToPositionIntent_TeleportToPositionGoal_Resolver>();
    }
}