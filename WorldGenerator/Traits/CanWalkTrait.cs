using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class CanWalkTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _aiTrait.RegisterIntentResolver<GoToPositionIntent_WalkToPositionGoal_Resolver>();
    }

    public override void OnLose()
    {
        _aiTrait.DeregisterIntentResolver<GoToPositionIntent_WalkToPositionGoal_Resolver>();
    }
}
