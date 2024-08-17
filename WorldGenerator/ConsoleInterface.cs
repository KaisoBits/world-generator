using System.Reflection;
using System.Text;
using System.Threading.Channels;
using WorldGenerator.AI;
using WorldGenerator.Events;
using WorldGenerator.Factories;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class ConsoleInterface
{
    private readonly World _world;
    private readonly EntityFactory _entityFactory;
    private readonly EventBus _eventBus;
    private readonly SelectionService _selectionService;
    private readonly JobOrderManager _workOrderManager;

    private readonly IEnumerable<Type> _traitTypes =
        Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(t => t.IsAssignableTo(typeof(ITrait)));

    private readonly Dictionary<string, Func<IReadOnlyList<string>, string?>> _commands = [];

    private readonly Channel<string> _commandChannel = Channel.CreateUnbounded<string>();

    public ConsoleInterface(World world, EntityFactory entityFactory, EventBus eventBus, SelectionService selectionService, JobOrderManager workOrderManager)
    {
        _world = world;
        _entityFactory = entityFactory;
        _eventBus = eventBus;
        _selectionService = selectionService;
        _workOrderManager = workOrderManager;

        RegisterCommand("help", Help);
        RegisterCommand("clear", Clear);
        RegisterCommand("events", DisplayEvents);
        RegisterCommand("tile", DisplayTileInfo);
        RegisterCommand("mine", Mine);
        RegisterCommand("spawn", Spawn);
        RegisterCommand("addtrait", AddTrait);
        RegisterCommand("remtrait", RemoveTrait);
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
            try
            {
                RunCommand(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Critical fail: " + ex.Message);
            }
        }
    }

    public void RunCommand(string fullCommand)
    {
        string[] parts = fullCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return;

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

    public void RegisterCommand(string name, Func<IReadOnlyList<string>, string?> handler)
    {
        _commands.Add(name, handler);
    }

    public string? Help(IReadOnlyList<string> parameters)
    {
        Console.WriteLine("Available commands:");
        foreach (var command in _commands)
            Console.WriteLine($"  - {command.Key}");

        return null;
    }

    public string? Clear(IReadOnlyList<string> parameters)
    {
        Console.Clear();

        return null;
    }

    public string? DisplayEvents(IReadOnlyList<string> parameters)
    {
        IEnumerable<GameEvent> listBottom = _eventBus.EventList.TakeLast(20);
        foreach (var item in listBottom)
            Console.WriteLine(item);

        return null;
    }

    public string? Mine(IReadOnlyList<string> parameters)
    {
        ITileView? tile = _selectionService.SelectedTile;
        if (tile == null)
            return "No tile selected";

        _workOrderManager.ScheduleMineJob(tile);

        return null;
    }

    public string? DisplayTileInfo(IReadOnlyList<string> parameters)
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

        if (entity.TryGetTrait(out GoalTrait? goalTrait))
        {
            Console.WriteLine("Goal:");

            if (goalTrait.CurrentGoal != null)
            {
                StringBuilder sb = new();
                IGoal? currGoal = goalTrait.CurrentGoal;

                sb.Append(currGoal.GetType().Name);
                currGoal = currGoal.InterruptedWith;
                while (currGoal != null)
                {
                    sb.Append(" -> ");
                    sb.Append(currGoal.GetType().Name);
                    currGoal = currGoal.InterruptedWith;
                }

                Console.WriteLine($"  Goal: {sb.ToString() ?? "-"}");
                Console.WriteLine($"  State: {currGoal?.State.ToString() ?? "-"}");

            }
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

    public string? Spawn(IReadOnlyList<string> parameters)
    {
        if (parameters.Count != 2)
            return "Pass the name of the entity to add";

        ITileView? tile = _selectionService.SelectedTile;
        if (tile == null)
            return "No tile selected";

        Entity entity = _entityFactory.CreateFromName(parameters[1]);

        _world.SpawnEntity(entity, tile.Position);

        return null;
    }

    public string? AddTrait(IReadOnlyList<string> parameters)
    {
        if (parameters.Count != 2)
            return "Pass the name of the trait to add";

        ITileView? tile = _selectionService.SelectedTile;
        if (tile == null)
            return "No tile selected";

        IEntity? ent = tile.Contents.LastOrDefault();
        if (ent == null)
            return "No entity on selected tile";

        Type? traitType = _traitTypes.FirstOrDefault(t => t.Name.Equals(parameters[1], StringComparison.OrdinalIgnoreCase));

        if (traitType == null)
            return "Trait does not exist";

        MethodInfo? methodInfo = typeof(IEntity).GetMethod(nameof(IEntity.AddTrait), BindingFlags.Public | BindingFlags.Instance);
        if (methodInfo != null)
        {
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(traitType);
            genericMethod.Invoke(ent, null);
        }

        return null;
    }

    public string? RemoveTrait(IReadOnlyList<string> parameters)
    {
        if (parameters.Count != 2)
            return "Pass the name of the trait to remove";

        ITileView? tile = _selectionService.SelectedTile;
        if (tile == null)
            return "No tile selected";

        IEntity? ent = tile.Contents.LastOrDefault();
        if (ent == null)
            return "No entity on selected tile";

        Type? traitType = _traitTypes.FirstOrDefault(t => t.Name.Equals(parameters[1], StringComparison.OrdinalIgnoreCase));

        if (traitType == null)
            return "Trait does not exist";

        MethodInfo? methodInfo = typeof(IEntity).GetMethod(nameof(IEntity.RemoveTrait), BindingFlags.Public | BindingFlags.Instance);
        if (methodInfo != null)
        {
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(traitType);
            genericMethod.Invoke(ent, null);
        }

        return null;
    }
}