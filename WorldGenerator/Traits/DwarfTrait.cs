//using WorldGenerator.AI;
//using WorldGenerator.Factories;
//using WorldGenerator.Memories;
//using WorldGenerator.Moodlets;
//using WorldGenerator.States;

//namespace WorldGenerator.Traits;

//public sealed class DwarfTrait : Trait<DwarfTrait.DataType>
//{
//    private readonly World _world;
//    private readonly GoalFactory _goalFactory;

//    private MoodTrait _moodTrait = default!;
//    private MemoryTrait _memoryTrait = default!;
//    private AITrait _aiTrait = default!;

//    public DwarfTrait(World world, GoalFactory goalFactory)
//    {
//        _world = world;
//        _goalFactory = goalFactory;
//    }

//    protected override void OnGain()
//    {
//        _moodTrait = RequireTrait<MoodTrait>();
//        _memoryTrait = RequireTrait<MemoryTrait>();
//        _aiTrait = RequireTrait<AITrait>();
//    }

//    public override void OnGatherConditions()
//    {
//        int health = Owner.GetState<HealthState>()?.Health ?? 0;
//        if (health <= 0)
//            Owner.ApplyMoodlet<DeadMoodlet>();
//        else
//            Owner.RemoveMoodlet<DeadMoodlet>();

//        Owner.RemoveMoodlet<JustEnteredFortressMoodlet>();

//        if (Owner.CurrentTile.Contents.Any(e => e.EntityType.Matches("*.building.*")))
//        {
//            if (!Owner.HasMoodlet<InBuildingMoodlet>())
//            {
//                Owner.ApplyMoodlet<InBuildingMoodlet>();
//                Owner.ApplyMoodlet<JustEnteredFortressMoodlet>();
//            }
//        }
//        else
//        {
//            Owner.RemoveMoodlet<InBuildingMoodlet>();
//        }
//    }

//    public override void Tick()
//    {
//        if (Owner.HasMoodlet<JustEnteredFortressMoodlet>())
//        {
//           _memoryTrait.Remember(
//                new VisitedFortressMemory(
//                    Owner.CurrentTile.Contents
//                    .FirstOrDefault(c => c.EntityType.Matches("*.building.*"))
//                    ?.GetState<NameState>()?.Name ?? string.Empty));
//        }

//        //if (_aiTrait.CurrentGoal != null)
//        //    return;

//        bool shouldTravel = Random.Shared.NextDouble() < Data.Chance;
//        if (!shouldTravel)
//            return;

//        //if (Owner.HasMoodlet<InBuildingMoodlet>())
//        //    _aiTrait.AssignScheduler(_schedulerFactory.CreateScheduler<GoToRandomBuildingGoal>());
//        //else
//        //    _aiTrait.AssignScheduler(_schedulerFactory.CreateScheduler<GoToNearestBuildingGoal>());
//    }

//    public record class DataType
//    {
//        public float Chance { get; init; } = 0.01f;
//    }
//}
