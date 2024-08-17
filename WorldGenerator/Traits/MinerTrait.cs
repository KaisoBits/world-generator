using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class MinerTrait : Trait<NullTraitData>
{
    private readonly IntentResolverFactory _intentResolverFactory;

    private MineBlockIntent_MineBlockGoal_Resolver? _resolver;

    public MinerTrait(IntentResolverFactory intentResolverFactory)
    {
        _intentResolverFactory = intentResolverFactory;
    }

    protected override void OnGain()
    {
        _resolver = _intentResolverFactory.CreateResolver<MineBlockIntent_MineBlockGoal_Resolver>();
        Owner.AddToList<IIntentResolver>(_resolver);
    }

    public override void OnLose()
    {
        if (_resolver != null)
            Owner.RemoveFromList<IIntentResolver>(_resolver);
    }
}
