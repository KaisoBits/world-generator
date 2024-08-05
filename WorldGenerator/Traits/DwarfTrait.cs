using WorldGenerator.AI;
using WorldGenerator.Factories;
using WorldGenerator.Memories;
using WorldGenerator.Moodlets;
using WorldGenerator.States;

namespace WorldGenerator.Traits;

public sealed class DwarfTrait : Trait<DwarfTrait.DataType>
{
    private readonly World _world;
    private readonly SchedulerFactory _schedulerFactory;

    private MoodTrait _moodTrait = default!;
    private MemoryTrait _memoryTrait = default!;
    private AITrait _aiTrait = default!;

    public DwarfTrait(World world, SchedulerFactory schedulerFactory)
    {
        _world = world;
        _schedulerFactory = schedulerFactory;
    }

    protected override void OnGain()
    {
        _moodTrait = RequireTrait<MoodTrait>();
        _memoryTrait = RequireTrait<MemoryTrait>();
        _aiTrait = RequireTrait<AITrait>();
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
            _moodTrait.ApplyMoodlet<InBuildingMoodlet>(_world.CurrentTick + 5);

        if (Owner.InCondition<JustEnteredBuildingCondition>())
        {
           _memoryTrait.Remember(
                new VisitedBuildingMemory(
                    Owner.CurrentTile.Contents
                    .FirstOrDefault(c => c.Layer == Layer.Buildings)
                    ?.GetState<NameState>()?.Name ?? string.Empty));
        }

        if (_aiTrait.CurrentScheduler != null)
            return;

        bool shouldTravel = Random.Shared.NextDouble() < Data.Chance;
        if (!shouldTravel)
            return;

        if (Owner.InCondition<InBuildingCondition>())
            _aiTrait.AssignScheduler(_schedulerFactory.CreateScheduler<GoToRandomBuildingScheduler>());
        else
            _aiTrait.AssignScheduler(_schedulerFactory.CreateScheduler<GoToNearestBuildingScheduler>());
    }

    public record class DataType
    {
        public float Chance { get; init; } = 0.1f;
    }
}
