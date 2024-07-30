﻿using WorldGenerator.AI;
using WorldGenerator.States;

namespace WorldGenerator.Traits;

public class TravelerTrait : Trait<TravelerTrait.Data>
{
    public TravelerTrait(Data data) : base(data)
    {
    }

    public override void Tick()
    {
        if (Owner is not Creature creature)
            return;

        if (creature.CurrentScheduler != null)
            return;

        bool shouldTravel = Random.Shared.NextDouble() < TraitData.Chance;
        if (!shouldTravel)
            return;
        
        if (creature.InCondition<InBuildingCondition>())
            creature.AssignScheduler(new GoToRandomBuildingScheduler());
        else
            creature.AssignScheduler(new GoToNearestBuildingScheduler());
    }

    public record class Data(float Chance);
}
