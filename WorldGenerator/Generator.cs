using SFML.System;

namespace WorldGenerator;

public class Generator
{
    public void PopulateTiles(Tile[,] tiles)
    {
        List<Building> buildings = GenerateBuildings(10, tiles);
    }

    private List<Building> GenerateBuildings(int count, Tile[,] tiles)
    {
        Vector2i[] positions = new Vector2i[tiles.Length];
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                positions[j * tiles.GetLength(0) + i] = new Vector2i(i, j);
            }
        }
        Vector2i[] buildingPositions = Random.Shared.GetItems(positions, count);
        List<Building> result = [];

        foreach (var pos in buildingPositions)
        {
            Building building = new();
            tiles[pos.X, pos.Y].AddEntity(building);
            result.Add(building);
        }

        return result;
    }
}
