﻿using Microsoft.Extensions.DependencyInjection;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator.Factories;

public sealed class EntityFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TraitFactory _traitFactory;

    public EntityFactory(IServiceProvider serviceProvider, TraitFactory traitFactory)
    {
        _serviceProvider = serviceProvider;
        _traitFactory = traitFactory;
    }

    // Placeholder
    public Entity CreateFromName(string name)
    {
        Entity result = ActivatorUtilities.CreateInstance<Entity>(_serviceProvider);
        switch (name)
        {
            case "dwarf":
                result.AddTrait<MoodTrait>();
                result.AddTrait<MemoryTrait>();
                result.AddTrait<AITrait>();
                result.AddTrait<DwarfTrait>().WithData(new() { Chance = 0.015f });
                result.SetState(new HealthState(100));
                result.SetState(new SpeedState(Random.Shared.Next(1, 21)));
                result.SetState(new NameState(NameGenerator.GetDwarfName()));
                result.Layer = Layer.Creatures;
                result.RenderType = "dwarf";
                break;
            case "fortress":
                result.SetState(new NameState(NameGenerator.GetFortressName()));
                result.Layer = Layer.Buildings;
                result.RenderType = "building";
                break;
            case "mountain":
                result.Layer = Layer.Ground;
                result.RenderType = "mountain";
                break;
            default:
                throw new Exception("Unknown entity type " + name);
        }

        return result;
    }
}
