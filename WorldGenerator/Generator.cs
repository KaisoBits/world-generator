namespace WorldGenerator;

public class Generator
{
    public void PopulateWorld(World world, int buildingCount)
    {
        List<Building> buildings = GenerateBuildings(buildingCount, world);
    }

    private List<Building> GenerateBuildings(int count, World world)
    {
        (int X, int Y)[] positions = world
            .Where(t => t.Contents is [])
            .Select(t => (t.X, t.Y))
            .ToArray();


        (int X, int Y)[] buildingPositions = Random.Shared.GetItems(positions, count);
        List<Building> result = [];

        foreach (var (x, y) in buildingPositions)
        {
            Building building = Building.EstablishCity("Forteca p.w. " + NameGenerator.GetDwarfName());
            world.SpawnEntity(building, x, y);
            result.Add(building);
        }

        return result;
    }
}
