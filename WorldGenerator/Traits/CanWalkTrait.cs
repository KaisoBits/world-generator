using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class CanWalkTrait : Trait<NullTraitData>
{
    private GoalTrait _goalTrait = default!;

    protected override void OnGain()
    {
        _goalTrait = RequireTrait<GoalTrait>();

        _goalTrait.RegisterIntentResolver<GoToPositionIntent_WalkToPositionGoal_Resolver>();
    }

    public override void OnLose()
    {
        _goalTrait.DeregisterIntentResolver<GoToPositionIntent_WalkToPositionGoal_Resolver>();
    }
}
