using WorldGenerator.AI;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class ConsoleInterface
{
    private readonly EventBus _eventBus;

    public ConsoleInterface(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void StartDisplayingEvents()
    {
        var listBottom = _eventBus.EventList.TakeLast(20);
        foreach (var item in listBottom)
            Console.WriteLine(item);

        //Subscription sub = EventBus.Subscribe<GameEvent>(Console.WriteLine);
    }

    public void DisplayTileInfo(ITileView tile)
    {
        Console.WriteLine($"Tile {tile.Position}:");
        foreach (IEntity entity in tile.Contents)
        {
            Console.WriteLine("*******************************************************************************************************************");
            Console.WriteLine();
            DisplayEntityInfo(entity);
            Console.WriteLine();
        }
        Console.WriteLine("*******************************************************************************************************************");
    }

    public void DisplayEntityInfo(IEntity entity)
    {
        Console.Write($"Entity: {entity.GetState<NameState>()?.Name} ({entity.ID})");

        Console.WriteLine();
        Console.WriteLine($"Layer: {entity.Layer}");
        Console.WriteLine($"Entity Type: {entity.EntityType ?? "-"}");
        Console.WriteLine("---------------------------------------");

        Console.WriteLine("Moodlets:");

        foreach (var item in entity.Moodlets)
        {
            Console.Write("  ");
            Console.WriteLine($"- {item.Name} ({item.Description})");
        }
        Console.WriteLine("---------------------------------------");


        Console.WriteLine($"Traits: {string.Join(", ", entity.Traits.Select(t => t.GetType().Name))}");

        Console.WriteLine("---------------------------------------");

        if (entity.TryGetTrait(out AITrait? aiTrait))
        {
            Console.WriteLine("AI:");

            Scheduler? scheduler = aiTrait.CurrentScheduler as Scheduler;
            Console.WriteLine($"  Scheduler: {scheduler?.GetType().Name ?? "-"}");
            Console.WriteLine($"  State: {scheduler?.State.ToString() ?? "-"}");
            Console.WriteLine($"  Current Task: {scheduler?.CurrentTask?.GetType().Name.ToString() ?? "-"}");

            Console.WriteLine("---------------------------------------");
        }

        Console.WriteLine("States:");

        foreach (var item in entity.States)
        {
            Console.Write("  ");
            Console.WriteLine($"- {item}");
        }

        if (entity.TryGetTrait(out MemoryTrait? memoryTrait))
        {
            Console.WriteLine("---------------------------------------");


            Console.WriteLine("Memories:");

            if (memoryTrait.Memories.Count > 10)
                Console.WriteLine($"  ...{memoryTrait.Memories.Count - 10} more memories");
            foreach (var item in memoryTrait.Memories.TakeLast(10))
            {
                Console.WriteLine($"  - {item.Message}");
            }
        }
    }
}
