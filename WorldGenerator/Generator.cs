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
        (int X, int Y)[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is[])
            return [];

        (int X, int Y)[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<Building> result = [];

        foreach (var (x, y) in buildingPositions)
        {
            Building building = Building.EstablishCity("Forteca p.w. " + NameGenerator.GetDwarfName());
            world.SpawnEntity(building, x, y);
            result.Add(building);
        }

        return result;
    }

    private List<Entity> GenerateCitizens(int count, World world)
    {
        (int X, int Y)[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is [])
            return [];

        (int X, int Y)[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<Entity> result = [];

        foreach (var (x, y) in buildingPositions)
        {
            Entity ent = new Creature();
            world.SpawnEntity(ent, x, y);
            result.Add(ent);
        }

        return result;
    }

    private (int X, int Y)[] GetEmptyPositions(World world)
    {
        (int X, int Y)[] positions = world
            .Where(t => t.Contents is [])
            .Select(t => (t.X, t.Y))
            .ToArray();

        return positions;
    }
}
