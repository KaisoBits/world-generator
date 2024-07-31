using WorldGenerator.AI;
using WorldGenerator.EntityExtensions;
using WorldGenerator.Factories;
using WorldGenerator.Memories;
using WorldGenerator.Moodlets;
using WorldGenerator.States;

namespace WorldGenerator.Traits;

public sealed class DwarfTrait : Trait<DwarfTrait.Data>
{
    private readonly World _world;
    private readonly SchedulerFactory _schedulerFactory;

    public DwarfTrait(World world, SchedulerFactory schedulerFactory)
    {
        _world = world;
        _schedulerFactory = schedulerFactory;
    }

    protected override void OnGain()
    {
        Owner.EnableExtension<MoodExtension>();
        Owner.EnableExtension<MemoryExtension>();
    }

    public override void OnGatherConditions()
    {
        int health = Owner.GetState<HealthState>()?.Health ?? 0;
        if (health <= 0)
            Owner.SetCondition<DeadCondition>();
        else
            Owner.ClearCondition<DeadCondition>();

        Owner.ClearCondition<JustEnteredBuildingCondition>();

        if (Owner.CurrentTile.Contents.Any(e => e.Layer == Layer.Buildings))
        {
            if (!Owner.InCondition<InBuildingCondition>())
            {
                Owner.SetCondition<InBuildingCondition>();
                Owner.SetCondition<JustEnteredBuildingCondition>();
            }
        }
        else
        {
            Owner.ClearCondition<InBuildingCondition>();
        }
    }

    public override void Tick()
    {
        if (Owner.InCondition<InBuildingCondition>())
            Owner.GetExtension<MoodExtension>().ApplyMoodlet<InBuildingMoodlet>(_world.CurrentTick + 5);

        if (Owner.InCondition<JustEnteredBuildingCondition>())
        {
            Owner.GetExtension<MemoryExtension>().Remember(
                new VisitedBuildingMemory(
                    Owner.CurrentTile.Contents
                    .FirstOrDefault(c => c.Layer == Layer.Buildings)
                    ?.GetState<NameState>()?.Name ?? string.Empty));
        }

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
