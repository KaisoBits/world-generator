using WorldGenerator.Factories;

namespace WorldGenerator;

public class Terrain
{
    public Terrain(World world, EntityFactory entityFactory)
    {
        for (int y = 0; y < world.Height; y++)
        {

            for (int x = 0; x < world.Width; x++)
            {
                //ITileView t = World.Instance[0, 0];
                int TerrainRand = Random.Shared.Next(0, 100);

                if (TerrainRand == 1)
                {
                    world.SpawnEntity(entityFactory.CreateFromName("mountain"), new Vector(x, y));
                    //Pathfinding.ValueGrid[x, y] = Pathfinding.ValueGrid[x, y] + 5;
                    // 5 is the terrain difficulty of main mountain, might be adjusted accordingly in later iterations.

                }
            }
        }
    }

    //public SpawnMountainChain(x, y) {

    //    if (int rnd = Random.Shared.Next(0, 1); rnd = 1){ 
        
        
    //    }
    
    //}


}

