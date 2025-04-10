﻿using System.Diagnostics.CodeAnalysis;
using WorldGenerator.Moodlets;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IEntity
{
    int ID { get; }

    IReadOnlyList<Moodlet> Moodlets { get; }
    IReadOnlyCollection<IState> States { get; }
    IReadOnlyCollection<ITrait> Traits { get; }

    Layer Layer { get; }

    EntityType EntityType { get; }

    public Vector Position { get; }

    ITileView CurrentTile { get; }

    bool IsSpawned { get; }

    void Think();

    void ApplyMoodlet<T>() where T : Moodlet;
    void ApplyMoodlet<T>(int expireOn) where T : Moodlet;
    bool HasMoodlet<T>() where T : Moodlet;
    bool RemoveMoodlet<T>() where T : Moodlet;

    void SetState<T>(T data) where T : struct, IState;
    T? GetState<T>() where T : struct, IState;

    void AddToList<T>(T item) where T : notnull;
    IEnumerable<T> GetList<T>() where T : notnull;
    void RemoveFromList<T>(T item) where T : notnull;

    void AddTrait<T>() where T : ITrait;
    bool TryAddTrait<T>() where T : ITrait;
    void RemoveTrait<T>() where T : ITrait;
    bool TryRemoveTrait<T>() where T : ITrait;
    bool HasTrait<T>() where T : ITrait;
    T GetTrait<T>() where T : ITrait;
    bool TryGetTrait<T>([NotNullWhen(true)] out T? trait) where T : ITrait;

    void AddModifier<T>(Func<T, T> modifier);
    void RemoveModifier<T>(Func<T, T> modifier);
    T GetValueAfterModifiers<T>(T value);
}
