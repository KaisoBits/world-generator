using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public sealed class EntityFactory
{
    private readonly IServiceProvider _serviceProvider;

    private int _currentID = 1;

    public EntityFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Entity CreateFromName(string name)
    {
        Entity result = ActivatorUtilities.CreateInstance<Entity>(_serviceProvider);
        result.EntityType = EntityType.Parse(name);

        switch (name)
        {
            case "stock.creature.dwarf":
                result.SetState(new HealthState(100));
                result.SetState(new SpeedState(Random.Shared.Next(1, 21)));
                result.SetState(new NameState(NameGenerator.GetDwarfName()));

                result.AddTrait<MemoryTrait>();
                result.AddTrait<GoalTrait>();
                result.AddTrait<AITrait>();
                result.AddTrait<WorkerTrait>();
                result.AddTrait<MinerTrait>();
                result.AddTrait<CanWalkTrait>();
                //result.AddTrait<CanTeleport>();
                result.AddTrait<AgileTrait>();

                result.Layer = Layer.Creatures;
                break;
            case "stock.building.fortress":
                result.SetState(new NameState(NameGenerator.GetFortressName()));
                result.Layer = Layer.Buildings;
                break;
            case "stock.terrain.mountain":
                result.Layer = Layer.Ground;
                break;
            case "stock.terrain.field":
                result.Layer = Layer.Ground;
                break;
            case "stock.terrain.berries.full":
                result.Layer = Layer.Ground;
                break;
            case "stock.terrain.berries.empty":
                result.Layer = Layer.Ground;
                break;

            default:
                throw new Exception("Unknown entity type " + name);
        }

        result.ID = _currentID++;

        return result;
    }
}
