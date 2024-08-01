﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WorldGenerator;
using WorldGenerator.Factories;
using WorldGenerator.SFML;

const int worldX = 400;
const int worldY = 300;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddScoped(sp => World.CreateWorld(worldX, worldY));
builder.Services.AddScoped<EventBus>();
builder.Services.AddScoped<ConsoleInterface>();
builder.Services.AddScoped<Terrain>();
builder.Services.AddScoped<Pathfinding>();

builder.Services.AddScoped<EntityFactory>();
builder.Services.AddScoped<TraitFactory>();
builder.Services.AddScoped<SchedulerFactory>();
builder.Services.AddScoped<SchedulerTaskFactory>();
builder.Services.AddScoped<MoodletFactory>();
builder.Services.AddScoped<EntityExtensionFactory>();

builder.Services.AddScoped<Generator>();
builder.Services.AddScoped<IRenderer, SfmlRenderer>();
builder.Services.AddScoped<DebugOverlay>();

builder.Services.AddHostedService<GameHostedService>();

using IHost app = builder.Build();
app.Run();

