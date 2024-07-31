using WorldGenerator.AI;
using WorldGenerator.Factories;
using WorldGenerator.States;

namespace WorldGenerator.Traits;

public class DwarfTrait : Trait<DwarfTrait.Data>
{
    private readonly SchedulerFactory _schedulerFactory;

    public DwarfTrait(SchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override void Tick()
    {
        if (Owner.CurrentScheduler != null)
            return;

        bool shouldTravel = Random.Shared.NextDouble() < 0.1;
        if (!shouldTravel)
            return;
        
        if (Owner.InCondition<InBuildingCondition>())
            Owner.AssignScheduler(_schedulerFactory.CreateScheduler<GoToRandomBuildingScheduler>());
        else
            Owner.AssignScheduler(_schedulerFactory.CreateScheduler<GoToNearestBuildingScheduler>());
    }

    public record class Data(float Chance);
}
