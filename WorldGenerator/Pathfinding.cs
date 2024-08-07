namespace WorldGenerator;

public class Pathfinding
{
    public int[,] ValueGrid;
    private readonly World _world;
    public Pathfinding(World world)
    {
        _world = world;

        ValueGrid = new int[_world.Width, _world.Height];

        for (int y = 0; y < _world.Height; y++)
        {
            for (int x = 0; x < _world.Width; x++)
            {

                ValueGrid[x, y] = 1;
            }
        }
    }

    public int GetTerrainValue(int x, int y)
    {
        int TerrainValue = ValueGrid[x, y];

        return TerrainValue;
    }
    public int PathLength(int startX, int startY, int targetX, int targetY)
    {
        int PathLengthValue = Math.Abs(startX - targetX) + Math.Abs(startY - targetY);

        return PathLengthValue;
    }
}