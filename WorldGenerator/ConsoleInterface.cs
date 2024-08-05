using WorldGenerator.AI;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class ConsoleInterface
{
    public void StartDisplayingEvents()
    {
        var listBottom = EventBus.EventList.TakeLast(20);
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
        Console.Write($"Entity: {entity.GetState<NameState>()?.Name}");
        if (entity.TryGetTrait(out MoodTrait? moodTrait))
        {
            Console.Write($" ({moodTrait.MoodLevel})");
        }

        Console.WriteLine();
        Console.WriteLine($"Layer: {entity.Layer}");
        Console.WriteLine($"Render Actor: {entity.RenderActor?.GetType().Name}");
        Console.WriteLine("---------------------------------------");

        if (moodTrait != null)
        {
            Console.WriteLine("Moodlets:");

            foreach (var item in moodTrait.Moodlets)
            {
                Console.Write("  ");
                string sign = item.MoodModifier >= 0 ? "+" : "";
                Console.WriteLine($"- {item.Name} ({item.Description}): {sign}{item.MoodModifier}");
            }
            Console.WriteLine("---------------------------------------");
        }

        Console.WriteLine($"Conditions: {string.Join(", ", entity.Conditions)}");

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
