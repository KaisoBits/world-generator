using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public sealed class EntityFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TraitFactory _traitFactory;

    private int _currentID = 1;

    public EntityFactory(IServiceProvider serviceProvider, TraitFactory traitFactory)
    {
        _serviceProvider = serviceProvider;
        _traitFactory = traitFactory;
    }

    public Entity CreateFromName(string name)
    {
        Entity result = ActivatorUtilities.CreateInstance<Entity>(_serviceProvider);
        switch (name)
        {
            case "dwarf":
                result.SetState(new HealthState(100));
                result.SetState(new SpeedState(Random.Shared.Next(1, 21)));
                result.SetState(new NameState(NameGenerator.GetDwarfName()));

                result.AddTrait<MoodTrait>();
                result.AddTrait<MemoryTrait>();
                result.AddTrait<AITrait>();
                result.AddTrait<DwarfTrait>().WithData(new() { Chance = 0.015f });

                result.Layer = Layer.Creatures;
                result.EntityType = "dwarf";
                break;
            case "fortress":
                result.SetState(new NameState(NameGenerator.GetFortressName()));
                result.Layer = Layer.Buildings;
                result.EntityType = "fortress";
                break;
            case "mountain":
                result.Layer = Layer.Ground;
                result.EntityType = "mountain";
                break;
            case "field":
                result.Layer = Layer.Buildings;
                result.EntityType = "field";
                break;
            default:
                throw new Exception("Unknown entity type " + name);
        }

        result.ID = _currentID++;

        return result;
    }
}
