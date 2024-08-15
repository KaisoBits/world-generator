using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class MinerTrait : Trait<NullTraitData>
{
    private GoalTrait _goalTrait = default!;

    protected override void OnGain()
    {
        _goalTrait = RequireTrait<GoalTrait>();

        _goalTrait.RegisterIntentResolver<MineBlockIntent_MineBlockGoal_Resolver>();
    }

    public override void OnLose()
    {
        _goalTrait.DeregisterIntentResolver<MineBlockIntent_MineBlockGoal_Resolver>();
    }
}
