using WorldGenerator.Factories;
using WorldGenerator.States;

namespace WorldGenerator;

public class Terrain
{
    private readonly World _world;
    private readonly EntityFactory _entityFactory;
    private readonly Pathfinding _pathfinding;

    public Terrain(World world, EntityFactory entityFactory, Pathfinding pathfinding)
    {
        _world = world;
        _entityFactory = entityFactory;
        _pathfinding = pathfinding;
    }

    public void SpawnMountainMother()
    {
        for (int y = 0; y < _world.Height; y++)
        {
            for (int x = 0; x < _world.Width; x++)
            {
                //ITileView t = World.Instance[0, 0];
                int TerrainRand = Random.Shared.Next(0, 100);

                if (TerrainRand == 1)
                {
                    Entity ent = _entityFactory.CreateFromName("mountain");
                    ent.SetState(new SizeState(16));
                    _world.SpawnEntity(ent, new Vector(x, y));

                    _pathfinding.ValueGrid[x, y] = _pathfinding.ValueGrid[x, y] + 5;
                    // 5 is the terrain difficulty of main mountain, might be adjusted accordingly in later iterations.

                    SpawnMountainChain(x, y);
                }
            }
        }
    }

    public void SpawnMountainChain(int x, int y)
    {
        Entity ent = _entityFactory.CreateFromName("mountain");
        ent.SetState(new SizeState(8));
        //% of hill spawning to tweak in later iterations
        if (Random.Shared.Next(0, 10) < 8 && x != 0)
        {
            _world.SpawnEntity(ent, new Vector(x - 1, y));
            _pathfinding.ValueGrid[x - 1, y] = _pathfinding.ValueGrid[x - 1, y] + 3;
        }
        if (Random.Shared.Next(0, 10) < 8 && x != _world.Width - 1)
        {
            _world.SpawnEntity(ent, new Vector(x + 1, y));
            _pathfinding.ValueGrid[x + 1, y] = _pathfinding.ValueGrid[x + 1, y] + 3;
        }
        if (Random.Shared.Next(0, 10) < 8 && y != 0)
        {
            _world.SpawnEntity(ent, new Vector(x, y - 1));
            _pathfinding.ValueGrid[x, y - 1] = _pathfinding.ValueGrid[x, y - 1] + 3;
        }
        if (Random.Shared.Next(0, 10) < 8 && y != _world.Height - 1)
        {
            _world.SpawnEntity(ent, new Vector(x, y + 1));
            _pathfinding.ValueGrid[x, y + 1] = _pathfinding.ValueGrid[x, y + 1] + 3;
        }

    }

    public void VillageAddons()
    {
        Entity ent = _entityFactory.CreateFromName("field");

        for (int y = 0; y < _world.Height; y++)
        {
            for (int x = 0; x < _world.Width; x++)
            {
                ITileView tile = _world[x, y];

                if (tile.Contents.Any(e => e.EntityType == "fortress"))
                {
                    for (int addonY = y - 1; y - 1 >= 0 && y + 1 < _world.Height && addonY <= y + 1; addonY++)
                    {
                        for (int addonX = x - 1; x - 1 >= 0 && x + 1 < _world.Width && addonX <= x + 1; addonX++)
                        {
                            ITileView addonTile = _world[addonX, addonY];
                            if (Random.Shared.Next(0, 10) < 6 && addonTile.Contents.Count == 0  /*All(e => e.Layer != Layer.Buildings) && tile.Contents.All(e => e.EntityType != "mountain")*/)
                            {
                                _world.SpawnEntity(ent, new Vector(addonX, addonY));
                            }
                        }
                    }
                }
            }
        }
    }
}


//tile.Contents.Contains(_fortress)


//public void SpawnSmallHill(IEntity ground)
//{
//    SizeState? size = ground.GetState<SizeState>();

//    for (int y = 0; y < _world.Height; y++)
//    {
//        for (int x = 0; x < _world.Width; x++)
//        {
//            if (GetState.size = 8)
//            {
//            }


//        }
//    }
//}


//        switch (Random.Shared.Next(0, 4))
//        {
//            case 0:
//                _world.SpawnEntity(ent, new Vector(x - 1, y));
//                break;
//            case 1:
//                _world.SpawnEntity(ent, new Vector(x + 1, y));
//                break;
//            case 2:
//                _world.SpawnEntity(ent, new Vector(x, y - 1));
//                break;
//            case 3:
//                _world.SpawnEntity(ent, new Vector(x, y + 1));
//                break;
//            }

//    }
//}


//for (int y = 0; y<world.Height; y++)
//        {
//            for (int x = 0; x<world.Width; x++)
//            {

