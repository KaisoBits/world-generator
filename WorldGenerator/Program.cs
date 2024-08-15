using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WorldGenerator;
using WorldGenerator.Factories;
using WorldGenerator.SFML;

const int worldX = 256;
const int worldY = 256;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddScoped(sp => World.CreateWorld(worldX, worldY));
builder.Services.AddScoped<WorldFacade>();
builder.Services.AddScoped<EventBus>();
builder.Services.AddScoped<ConsoleInterface>();
builder.Services.AddScoped<Terrain>();
builder.Services.AddScoped<Pathfinding>();
builder.Services.AddScoped<WorkOrderManager>();

builder.Services.AddScoped<EntityFactory>();
builder.Services.AddScoped<TraitFactory>();
builder.Services.AddScoped<GoalFactory>();
builder.Services.AddScoped<MoodletFactory>();
builder.Services.AddScoped<IntentResolverFactory>();

builder.Services.AddScoped<Generator>();
builder.Services.AddScoped<IRenderer, SFMLRenderer>();
builder.Services.AddScoped<SelectionService>();
builder.Services.AddScoped<DebugOverlay>();

builder.Services.AddHostedService<GameHostedService>();

using IHost app = builder.Build();
app.Run();

