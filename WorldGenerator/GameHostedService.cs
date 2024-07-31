using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace WorldGenerator;

public class GameHostedService : BackgroundService
{
    private readonly World _world;
    private readonly Generator _generator;
    private readonly ConsoleInterface _consoleInterface;
    private readonly IRenderer _renderer;

    bool _running = true;

    public GameHostedService(World world, Generator generator, ConsoleInterface consoleInterface, IRenderer renderer)
    {
        _world = world;
        _generator = generator;
        _consoleInterface = consoleInterface;
        _renderer = renderer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<IEntity> entities = _generator.PopulateWorld(10, 10);

        //ci.StartDisplayingEvents();

        Stopwatch sw = new();
        sw.Start();

        while (!stoppingToken.IsCancellationRequested)
        {

            if (sw.Elapsed >= TimeSpan.FromSeconds(0.3))
            {
                sw.Restart();

                if (_running)
                    _world.Tick();

                Console.Clear();
                Console.WriteLine("Running: " + _running);
                _consoleInterface.DisplayMoodletsAndMemory(entities.First());
            }

            _renderer.Render();
        }

        return Task.CompletedTask;
    }
}
