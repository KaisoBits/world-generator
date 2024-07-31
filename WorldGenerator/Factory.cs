using WorldGenerator.RenderActors;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class Factory
{
    // Placeholder
    public static Entity CreateFromName(string name)
    {
        Entity result = new();
        switch (name)
        {
            case "dwarf":
                result.AddTrait(new DwarfTrait(new DwarfTrait.Data(0.1f)));
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
