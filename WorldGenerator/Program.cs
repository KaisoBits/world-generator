using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator;

const int worldX = 40;
const int worldY = 30;

bool running = true;

RenderWindow window = new(new VideoMode(worldX * 32, worldY * 32), "World generator");
window.Closed += (s, e) => window.Close();
window.Resized += (s, e) => window.SetView(new View(new Vector2f(e.Width / 2, e.Height / 2), new Vector2f(e.Width, e.Height)));
window.MouseButtonPressed += (s, e) =>
{
    if (e.Button == Mouse.Button.Left)
        running = !running;
};

World.CreateWorld(worldX, worldY);

Generator wg = new();
List<Entity> entities = wg.PopulateWorld(World.Instance, 10, 10);

ConsoleInterface ci = new();
//ci.StartDisplayingEvents();

IRenderer renderer = new SfmlRenderer(window);

Stopwatch sw = new();
sw.Start();

while (window.IsOpen)
{
    window.DispatchEvents();

    window.Clear(new Color(135, 206, 235));

    if (sw.Elapsed >= TimeSpan.FromSeconds(0.3))
    {
        if (running)
            World.Instance.Tick();

        sw.Restart();

        Console.Clear();
        Console.WriteLine("Running: " + running);
        ci.DisplayMoodletsAndMemory(entities.OfType<Creature>().First());
    }

    renderer.RenderWorld(World.Instance);
    window.Display();
}