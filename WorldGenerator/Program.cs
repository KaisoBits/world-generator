using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator;

const int worldX = 32;
const int worldY = 16;

RenderWindow window = new(new VideoMode(worldX * 32, worldY * 32), "World generator");
window.Closed += (s, e) => window.Close();
window.Resized += (s, e) => window.SetView(new View(new Vector2f(e.Width / 2, e.Height / 2), new Vector2f(e.Width, e.Height)));

World.CreateWorld(worldX, worldY);

Generator wg = new();
wg.PopulateWorld(World.Instance, 5, 5);

ConsoleInterface ci = new();
ci.StartDisplayingEvents();

IRenderer renderer = new SfmlRenderer(window);

Stopwatch sw = new();
sw.Start();

while (window.IsOpen)
{
    window.DispatchEvents();

    window.Clear(new Color(135, 206, 235));

    if (sw.Elapsed >= TimeSpan.FromSeconds(0.1))
    {
        //wg.PopulateWorld(World.Instance, 1);

        sw.Restart();
    }

    renderer.RenderWorld(World.Instance);
    window.Display();
}