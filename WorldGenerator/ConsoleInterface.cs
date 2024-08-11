using System.Threading.Channels;
using WorldGenerator.AI;
using WorldGenerator.Events;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class ConsoleInterface
{
    private readonly EventBus _eventBus;
    private readonly SelectionService _selectionService;

    private readonly Dictionary<string, Func<IReadOnlyCollection<string>, string?>> _commands = [];

    private readonly Channel<string> _commandChannel = Channel.CreateUnbounded<string>();

    public ConsoleInterface(EventBus eventBus, SelectionService selectionService)
    {
        _eventBus = eventBus;
        _selectionService = selectionService;

        RegisterCommand("help", Help);
        RegisterCommand("clear", Clear);
        RegisterCommand("events", DisplayEvents);
        RegisterCommand("tile", DisplayTileInfo);
    }

    public void StartTakingInput()
    {
        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                string input = Console.ReadLine()!;
                _commandChannel.Writer.TryWrite(input);
            }
        }, TaskCreationOptions.LongRunning);
    }

    public void ProcessCommands()
    {
        while (_commandChannel.Reader.TryRead(out string? item))
        {
            RunCommand(item);
        }
    }

    public void RunCommand(string fullCommand)
    {
        string[] parts = fullCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (_commands.TryGetValue(parts[0], out var handler))
        {
            string? response = handler(parts);
            if (response == null)
                Console.WriteLine("Command executed successfully");
            else
                Console.WriteLine("Command failed with message: '{0}'", response);
        }
        else
        {
            Console.WriteLine("Command '{0}' not found", parts[0]);
        }
    }

    public void RegisterCommand(string name, Func<IReadOnlyCollection<string>, string?> handler)
    {
        _commands.Add(name, handler);
    }

    public string? Help(IReadOnlyCollection<string> parameters)
    {
        Console.WriteLine("Available commands:");
        foreach (var command in _commands)
            Console.WriteLine($"  - {command.Key}");

        return null;
    }

    public string? Clear(IReadOnlyCollection<string> parameters)
    {
        Console.Clear();

        return null;
    }

    public string? DisplayEvents(IReadOnlyCollection<string> parameters)
    {
        IEnumerable<GameEvent> listBottom = _eventBus.EventList.TakeLast(20);
        foreach (var item in listBottom)
            Console.WriteLine(item);

        return null;
    }

    public string? DisplayTileInfo(IReadOnlyCollection<string> parameters)
    {
        ITileView? tile = _selectionService.SelectedTile;
        if (tile == null)
            return "No tile selected";

        Console.WriteLine($"Tile {tile.Position}:");
        foreach (IEntity entity in tile.Contents)
        {
            Console.WriteLine("*******************************************************************************************************************");
            Console.WriteLine();
            DisplayEntityInfo(entity);
            Console.WriteLine();
        }
        Console.WriteLine("*******************************************************************************************************************");

        return null;
    }

    public void DisplayEntityInfo(IEntity entity)
    {
        Console.Write($"Entity: {entity.GetState<NameState>()?.Name} ({entity.ID})");

        Console.WriteLine();
        Console.WriteLine($"Layer: {entity.Layer}");
        Console.WriteLine($"Entity Type: {entity.EntityType}");
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