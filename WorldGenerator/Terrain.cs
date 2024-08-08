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

        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                //ITileView t = World.Instance[0, 0];
                int TerrainRand = Random.Shared.Next(0, 100);

                if (TerrainRand == 1)
                {
                    Entity ent = entityFactory.CreateFromName("mountain");
                    ent.SetState(new SizeState(16));
                    world.SpawnEntity(ent, new Vector(x, y));

                    pathfinding.ValueGrid[x, y] = pathfinding.ValueGrid[x, y] + 5;
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
}


//    public void VillageAddons(World world, EntityFactory entityFactory, Tile tile) {

//        Entity ent = _entityFactory.CreateFromName("field");

//        for (int y = 0; y < world.Height; y++)
//        {
//            for (int x = 0; x < world.Width; x++)
//            {


//                if (tile.Contents.Contains(_fortress))
//                {
//                    for (int addonY = y - 1; addonY <= y + 1; addonY++) {
//                        for (int addonX = x - 1; addonX <= x + 1; addonX++) {

//                            if (tile.Contents == (null) && Random.Shared.Next(0, 10) < 6) {
//                                _world.SpawnEntity(ent, new Vector(addonX, addonY));
//                                }

//                             }


//                        }

//                     }

//                  }
//             }
//        }
//    //tile.Contents.Contains(_fortress)
//}
    
    
    
    
    
    
    
    
    
    
    
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