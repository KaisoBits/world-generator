namespace WorldGenerator;

public class WorldUtilities
{
    private readonly World _world;

    public WorldUtilities(World world)
    {
        _world = world;
    }

    public List<IEntity> GetAll(Predicate<IEntity> predicate)
    {
        return _world.Entities.Where(w => predicate(w)).ToList();
    }

    public List<IEntity> GetEntitiesInRange(Vector position, Vector rectangle)
    {
        List<IEntity> entities = [];
        for (int y = position.Y - rectangle.Y; y < position.Y + rectangle.Y; y++)
        {
            for (int x = position.X - rectangle.X; x < position.X + rectangle.X; x++)
            {
                entities.AddRange(_world[x, y].Contents);
            }
        }

        return entities;
    }

    public IEntity? GetClosest(Vector startPos, Predicate<IEntity> predicate)
    {
        return _world.Entities
            .OrderBy(e => (e.Position - startPos).SimpleLen())
            .FirstOrDefault(e => predicate(e));
    }
}
