namespace WorldGenerator;

public class Generator
{
    public void PopulateWorld(World world, int buildingCount, int citizenCount)
    {
        _ = GenerateBuildings(buildingCount, world);
        _ = GenerateCitizens(citizenCount, world);
    }

    private List<Building> GenerateBuildings(int count, World world)
    {
        Position[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is[])
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
