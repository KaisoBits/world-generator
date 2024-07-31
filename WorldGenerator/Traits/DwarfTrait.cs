using WorldGenerator.AI;
using WorldGenerator.States;

namespace WorldGenerator.Traits;

public class DwarfTrait : Trait<DwarfTrait.Data>
{
    public DwarfTrait(Data data) : base(data)
    {
    }

    public override void Tick()
    {
        if (Owner.CurrentScheduler != null)
            return;

        bool shouldTravel = Random.Shared.NextDouble() < TraitData.Chance;
        if (!shouldTravel)
            return;
        
        if (Owner.InCondition<InBuildingCondition>())
            Owner.AssignScheduler(new GoToRandomBuildingScheduler());
        else
            Owner.AssignScheduler(new GoToNearestBuildingScheduler());
    }

    public record class Data(float Chance);
}
