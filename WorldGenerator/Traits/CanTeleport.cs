using WorldGenerator.AI;
using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class CanTeleport : Trait<NullTraitData>
{
    private readonly IntentResolverFactory _intentResolverFactory;

    private GoToPositionIntent_TeleportToPositionGoal_Resolver? _resolver;

    public CanTeleport(IntentResolverFactory intentResolverFactory)
    {
        _intentResolverFactory = intentResolverFactory;
    }

    protected override void OnGain()
    {
        _resolver = _intentResolverFactory.CreateResolver<GoToPositionIntent_TeleportToPositionGoal_Resolver>();
        Owner.AddToList<IIntentResolver>(_resolver);
    }

    public override void OnLose()
    {
        if (_resolver != null)
            Owner.RemoveFromList<IIntentResolver>(_resolver);
    }
}
