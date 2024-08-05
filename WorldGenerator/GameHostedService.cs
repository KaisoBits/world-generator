using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace WorldGenerator;

public class GameHostedService : BackgroundService
{
    private readonly World _world;
    private readonly Generator _generator;
    private readonly IRenderer _renderer;

    public GameHostedService(World world, Generator generator, IRenderer renderer)
    {
        _world = world;
        _generator = generator;
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
            if (sw.Elapsed >= TimeSpan.FromSeconds(0.1))
            {
                sw.Restart();

                _world.Tick();
            }

            _renderer.Render();
        }

        return Task.CompletedTask;
    }
}
