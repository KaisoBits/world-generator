using WorldGenerator.Traits;

namespace WorldGenerator;

public class WorldFacade
{
    private readonly World _world;

    public WorldFacade(World world)
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
        for (int z = position.Z - rectangle.Z; z < position.Z + rectangle.Z; z++)
        {
            for (int y = position.Y - rectangle.Y; y < position.Y + rectangle.Y; y++)
            {
                for (int x = position.X - rectangle.X; x < position.X + rectangle.X; x++)
                {
                    entities.AddRange(_world[x, y, z].Contents);
                }
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

    public T? FindByTrait<T>() where T : ITrait
    {
        return _world.Entities
            .SelectMany(e => e.Traits)
            .OfType<T>()
            .FirstOrDefault();
    }
}
