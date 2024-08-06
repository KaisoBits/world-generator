using System.Diagnostics.CodeAnalysis;
using WorldGenerator.Moodlets;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IEntity
{
    IReadOnlyList<Moodlet> Moodlets { get; }
    IReadOnlyCollection<IValueWithModifiers> States { get; }
    IReadOnlyCollection<ITrait> Traits { get; }

    Layer Layer { get; }

    string? RenderType { get; set; }

    public Vector Position { get; }

    ITileView CurrentTile { get; }

    bool IsSpawned { get; }

    void GatherConditions();
    void Think();

    void ApplyMoodlet<T>() where T : Moodlet;
    void ApplyMoodlet<T>(int expireOn) where T : Moodlet;
    bool HasMoodlet<T>() where T : Moodlet;
    bool RemoveMoodlet<T>() where T : Moodlet;

    void SetState<T>(T data) where T : struct, IState;
    T? GetState<T>() where T : struct, IState;

    T GetOrAddTrait<T>() where T : ITrait;
    T AddTrait<T>() where T : ITrait;
    bool TryAddTrait<T>() where T : ITrait;
    bool HasTrait<T>() where T : ITrait;
    T GetTrait<T>() where T : ITrait;
    bool TryGetTrait<T>([NotNullWhen(true)] out T? trait) where T : ITrait;
}
