using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class MinerTrait : Trait<NullTraitData>
{
    private AITrait _aiTrait = default!;

    protected override void OnGain()
    {
        _aiTrait = RequireTrait<AITrait>();

        _aiTrait.RegisterIntentResolver<MineBlockIntent_MineBlockGoal_Resolver>();
    }

    public override void OnLose()
    {
        _aiTrait.DeregisterIntentResolver<MineBlockIntent_MineBlockGoal_Resolver>();
    }
}
