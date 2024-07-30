using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class Generator
{
    public List<Entity> PopulateWorld(World world, int buildingCount, int citizenCount)
    {
        _ = GenerateBuildings(buildingCount, world);
        return GenerateCitizens(citizenCount, world);
    }

    private List<Building> GenerateBuildings(int count, World world)
    {
        Position[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is [])
            return [];

        Position[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<Building> result = [];

        foreach (Position pos in buildingPositions)
        {
            Building building = Building.EstablishCity(NameGenerator.GetFortressName());
            world.SpawnEntity(building, pos);
            result.Add(building);
        }

        return result;
    }

    private List<Entity> GenerateCitizens(int count, World world)
    {
        Position[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is [])
            return [];

        Position[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<Entity> result = [];

        foreach (Position pos in buildingPositions)
        {
            Entity ent = new Creature();
            ent.SetState(new NameState(NameGenerator.GetDwarfName()));
            ent.SetState(new HealthState(100));
            ent.AddTrait(new TravelerTrait(new(0.1f)));
            world.SpawnEntity(ent, pos);
            result.Add(ent);
        }

        return result;
    }

    private Position[] GetEmptyPositions(World world)
    {
        Position[] positions = world
            .Where(t => t.Contents is [])
            .Select(t => t.Position)
            .ToArray();

        return positions;
    }
}
