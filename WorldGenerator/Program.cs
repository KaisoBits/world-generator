using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WorldGenerator;
using WorldGenerator.Factories;
using WorldGenerator.SFML;

const int worldX = 128;
const int worldY = 128;
const int worldZ = 10;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddScoped(sp => World.CreateWorld(worldX, worldY, worldZ));
builder.Services.AddScoped<WorldFacade>();
builder.Services.AddScoped<EventBus>();
builder.Services.AddScoped<ConsoleInterface>();
builder.Services.AddScoped<Terrain>();
builder.Services.AddScoped<Pathfinding>();
builder.Services.AddScoped<JobOrderManager>();

builder.Services.AddScoped<EntityFactory>();
builder.Services.AddScoped<TraitFactory>();
builder.Services.AddScoped<GoalFactory>();
builder.Services.AddScoped<MoodletFactory>();
builder.Services.AddScoped<IntentResolverFactory>();
builder.Services.AddScoped<DecisionFactory>();

builder.Services.AddScoped<Generator>();
builder.Services.AddScoped<IRenderer, IsometricRenderer>();
builder.Services.AddScoped<SelectionService>();
builder.Services.AddScoped<DebugOverlay>();

builder.Services.AddHostedService<GameHostedService>();

using IHost app = builder.Build();
app.Run();

