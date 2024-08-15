using WorldGenerator.Factories;

namespace WorldGenerator;

public class Generator
{
    private readonly World _world;
    private readonly EntityFactory _entityFactory;
    private readonly Terrain _terrain;

    public Generator(World world, EntityFactory entityFactory, Terrain terrain)
    {
        _world = world;
        _entityFactory = entityFactory;
        _terrain = terrain;
    }

    public void PopulateWorld(int buildingCount, int citizenCount)
    {
        GenerateCitizens(citizenCount);
        return;

        GenerateBuildings(buildingCount);
        _terrain.SpawnMountainMother();
        _terrain.VillageAddons();
        GenerateCitizens(citizenCount);
    }

    private List<IEntity> GenerateBuildings(int count)
    {
        Vector[] emptyPositions = GetEmptyPositions(_world);
        if (emptyPositions is [])
            return [];

        Vector[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<IEntity> result = [];

        foreach (Vector pos in buildingPositions)
        {
            Entity building = _entityFactory.CreateFromName("stock.building.fortress");
            _world.SpawnEntity(building, pos);
            result.Add(building);
        }

        return result;
    }

    private List<IEntity> GenerateCitizens(int count)
    {
        Vector[] emptyPositions = GetEmptyPositions(_world);
        if (emptyPositions is [])
            return [];

        Vector[] buildingPositions = Random.Shared.GetItems(emptyPositions, count);
        List<IEntity> result = [];

        foreach (Vector pos in buildingPositions)
        {
            Entity ent = _entityFactory.CreateFromName("stock.creature.dwarf");
            _world.SpawnEntity(ent, pos);
            result.Add(ent);
        }

        return result;
    }

    private Vector[] GetEmptyPositions(World world)
    {
        Vector[] positions = world
            .Where(t => !t.IsOccupied)
            .Select(t => t.Position)
            .ToArray();

        return positions;
    }
}
