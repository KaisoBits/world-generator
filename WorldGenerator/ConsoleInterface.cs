using WorldGenerator.AI;
using WorldGenerator.EntityExtensions;
using WorldGenerator.States;

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

    public void DisplayMoodletsAndMemory(IEntity entity)
    {
        Console.Write($"Creature {entity.GetState<NameState>()?.Name}");
        if (entity.TryGetExtension(out MoodExtension? moodExtension))
        {
            Console.Write($" ({moodExtension.MoodLevel})");
        }

        Console.WriteLine();
        Console.WriteLine("---------------------------------------");

        if (moodExtension != null)
        {
            Console.WriteLine("Moodlets:");

            foreach (var item in moodExtension.Moodlets)
            {
                Console.Write("  ");
                string sign = item.MoodModifier >= 0 ? "+" : "";
                Console.WriteLine($"- {item.Name} ({item.Description}): {sign}{item.MoodModifier}");
            }
            Console.WriteLine("---------------------------------------");
        }

        Console.WriteLine($"Conditions: {string.Join(",", entity.Conditions)}");

        Console.WriteLine("---------------------------------------");

        Console.WriteLine($"Traits: {string.Join(",", entity.Traits.Select(t => t.GetType().Name))}");

        Console.WriteLine("---------------------------------------");

        Console.WriteLine("AI DEBUG:");

        Scheduler? scheduler = entity.CurrentScheduler as Scheduler;
        Console.WriteLine("  Scheduler:");
        Console.WriteLine($"    Name: {scheduler?.GetType().Name ?? "None"}");
        Console.WriteLine($"    State: {scheduler?.State.ToString() ?? "-"}");
        Console.WriteLine();

        if (scheduler != null)
        {
            Console.WriteLine("  Tasks:");

            int i = 0;
            foreach (var item in scheduler.GetTasks())
            {
                Console.Write("    - " + item.GetType().Name);
                if (scheduler.CurrentTaskIndex == i)
                    Console.Write(" <<<");
                Console.WriteLine();

                i++;
            }
        }

        Console.WriteLine("---------------------------------------");

        Console.WriteLine("States:");

        foreach (var item in entity.States)
        {
            Console.Write("  ");
            Console.WriteLine($"- {item}");
        }

        Console.WriteLine("---------------------------------------");


        if (entity.TryGetExtension(out MemoryExtension? memoryExtension))
        {
            Console.WriteLine("Memories:");

            if (memoryExtension.Memories.Count > 10)
                Console.WriteLine($"  ...{memoryExtension.Memories.Count - 10} more memories");
            foreach (var item in memoryExtension.Memories.TakeLast(10))
            {
                Console.WriteLine($"  - {item.Message}");
            }
            Console.WriteLine("---------------------------------------");
        }
    }
}
