namespace WorldGenerator;

public class Generator
{
    public List<IEntity> PopulateWorld(World world, int buildingCount, int citizenCount)
    {
        _ = GenerateBuildings(buildingCount, world);

        return GenerateCitizens(citizenCount, world);
    }

    private List<IEntity> GenerateBuildings(int count, World world)
    {
        Vector[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is [])
            return [];

        Vector[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<IEntity> result = [];

        foreach (Vector pos in buildingPositions)
        {
            Entity building = Factory.CreateFromName("fortress");
            world.SpawnEntity(building, pos);
            result.Add(building);
        }

        return result;
    }

    private List<IEntity> GenerateCitizens(int count, World world)
    {
        Vector[] emptyPositions = GetEmptyPositions(world);
        if (emptyPositions is [])
            return [];

        Vector[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<IEntity> result = [];

        foreach (Vector pos in buildingPositions)
        {
            Entity ent = Factory.CreateFromName("dwarf");
            world.SpawnEntity(ent, pos);
            result.Add(ent);
        }

        return result;
    }

    private Vector[] GetEmptyPositions(World world)
    {
        Vector[] positions = world
            .Where(t => t.Contents is [])
            .Select(t => t.Position)
            .ToArray();

        return positions;
    }
}
