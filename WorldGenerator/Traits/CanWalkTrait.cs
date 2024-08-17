using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class CanWalkTrait : Trait<NullTraitData>
{
    private readonly IntentResolverFactory _intentResolverFactory;

    private GoToPositionIntent_WalkToPositionGoal_Resolver? _resolver;

    public CanWalkTrait(IntentResolverFactory intentResolverFactory)
    {
        _intentResolverFactory = intentResolverFactory;
    }

    protected override void OnGain()
    {
        _resolver = _intentResolverFactory.CreateResolver<GoToPositionIntent_WalkToPositionGoal_Resolver>();
        Owner.AddToList<IIntentResolver>(_resolver);
    }

    public override void OnLose()
    {
        if (_resolver != null)
            Owner.RemoveFromList<IIntentResolver>(_resolver);
    }
}
