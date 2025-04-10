﻿using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace WorldGenerator;

public class GameHostedService : BackgroundService
{
    private readonly World _world;
    private readonly JobOrderManager _workOrderManager;
    private readonly Generator _generator;
    private readonly IRenderer _renderer;

    public GameHostedService(World world, JobOrderManager workOrderManager, Generator generator, IRenderer renderer)
    {
        _world = world;
        _workOrderManager = workOrderManager;
        _generator = generator;
        _renderer = renderer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _generator.PopulateWorld(10, 200);

        //ci.StartDisplayingEvents();

        Stopwatch sw = new();
        sw.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (sw.Elapsed >= TimeSpan.FromSeconds(0.1))
            {
                sw.Restart();

                _world.Tick();
                _workOrderManager.Tick();
            }

            _renderer.Render();
        }

        return Task.CompletedTask;
    }
}
