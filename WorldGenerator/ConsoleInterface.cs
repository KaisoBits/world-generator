using WorldGenerator.AI;
using WorldGenerator.States;

namespace WorldGenerator;

public class ConsoleInterface
{
    public void StartDisplayingEvents()
    {
        var listBottom = EventBus.EventList.TakeLast(20);
        foreach (var item in listBottom)
            Console.WriteLine(item);

        Subscription sub = EventBus.Subscribe<GameEvent>(Console.WriteLine);
    }

    public void DisplayMoodletsAndMemory(Creature creature)
    {
        Console.WriteLine($"Creature {creature.GetState<NameState>()?.Name} ({creature.MoodLevel})");
        Console.WriteLine("---------------------------------------");

        Console.WriteLine("Moodlets:");

        foreach (var item in creature.Moodlets)
        {
            Console.Write("  ");
            string sign = item.MoodModifier >= 0 ? "+" : "";
            Console.WriteLine($"- {item.Name} ({item.Description}): {sign}{item.MoodModifier}");
        }
        Console.WriteLine("---------------------------------------");

        Console.WriteLine($"Conditions: {string.Join(",", creature.Conditions)}");

        Console.WriteLine("---------------------------------------");

        Console.WriteLine($"Traits: {string.Join(",", creature.Traits.Select(t => t.GetType().Name))}");

        Console.WriteLine("---------------------------------------");

        Console.WriteLine("AI DEBUG:");

        Scheduler? scheduler = creature.CurrentScheduler as Scheduler;
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

        foreach (var item in creature.States)
        {
            Console.Write("  ");
            Console.WriteLine($"- {item}");
        }

        Console.WriteLine("---------------------------------------");

        Console.WriteLine("Memories:");

        if (creature.Memories.Count > 10)
            Console.WriteLine($"  ...{creature.Memories.Count - 10} more memories");
        foreach (var item in creature.Memories.TakeLast(10))
        {
            Console.WriteLine($"  - {item.Message}");
        }
        Console.WriteLine("---------------------------------------");
    }
}
