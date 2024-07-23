using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator;

RenderWindow window = new(new VideoMode(800, 600), "World generator");
window.Closed += (s, e) => window.Close();
window.Resized += (s, e) => window.SetView(new View(new Vector2f(e.Width / 2, e.Height / 2), new Vector2f(e.Width, e.Height)));

Tile[,] tiles = new Tile[24, 18];
for (int i = 0; i < tiles.GetLength(0); i++)
    for (int j = 0; j < tiles.GetLength(1); j++)
        tiles[i, j] = new Tile();

Generator wg = new();
wg.PopulateTiles(tiles, 5);

ConsoleInterface ci = new();
ci.StartDisplayingEvents();

Renderer rendered = new(window);

Stopwatch sw = new();
sw.Start();

while (window.IsOpen)
{
    window.DispatchEvents();

    window.Clear(new Color(135, 206, 235));

    if (sw.Elapsed >= TimeSpan.FromSeconds(1))
    {
        wg.PopulateTiles(tiles, 1);
        sw.Restart();
    }

    rendered.RenderTiles(tiles);
    window.Display();
}