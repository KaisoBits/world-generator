using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.RenderActors;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public sealed class EntityFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TraitFactory _traitFactory;

    public EntityFactory(IServiceProvider serviceProvider, TraitFactory traitFactory)
    {
        _serviceProvider = serviceProvider;
        _traitFactory = traitFactory;
    }

    // Placeholder
    public Entity CreateFromName(string name)
    {
        Entity result = ActivatorUtilities.CreateInstance<Entity>(_serviceProvider);
        switch (name)
        {
            case "dwarf":
                result.AddTrait(_traitFactory.CreateTrait<DwarfTrait>());
                result.SetState(new HealthState(100));
                result.SetState(new NameState(NameGenerator.GetDwarfName()));
                result.Layer = Layer.Creatures;
                result.RenderActor = DwarfRenderActor.Instance;
                break;
            case "fortress":
                result.SetState(new NameState(NameGenerator.GetFortressName()));
                result.Layer = Layer.Buildings;
                result.RenderActor = BuildingRenderActor.Instance;
                break;
            default:
                throw new Exception("Unknown entity type " + name);
        }

        return result;
    }
}
