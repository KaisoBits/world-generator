using System.Diagnostics.CodeAnalysis;
using WorldGenerator.Factories;
using WorldGenerator.Moodlets;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public sealed class Entity : IEntity
{
    public int ID { get; internal set; }

    public IReadOnlyList<Moodlet> Moodlets => _moodlets;
    private readonly List<Moodlet> _moodlets = [];

    public IReadOnlyCollection<IState> States => _states.Values;
    private readonly Dictionary<Type, IState> _states = [];

    public IReadOnlyCollection<ITrait> Traits => _traits;
    private readonly List<ITrait> _traits = [];

    private readonly Dictionary<Type, List<object>> _modifiers = [];

    public EntityType EntityType { get; internal set; } = EntityType.Parse("unassigned.unassigned.unassigned");

    public Vector Position { get; internal set; }

    public ITileView CurrentTile => _world[Position];

    public Layer Layer { get; set; }
    public bool IsSpawned { get; private set; }

    private readonly World _world;
    private readonly TraitFactory _traitFactory;
    private readonly MoodletFactory _moodletFactory;

    public Entity(World world, TraitFactory traitFactory, MoodletFactory moodletFactory)
    {
        _world = world;
        _traitFactory = traitFactory;
        _moodletFactory = moodletFactory;
    }

    public void Think()
    {
        RemoveExpiredMoodlets();

        foreach (Moodlet moodlet in _moodlets)
            moodlet.Tick();

        foreach (ITrait trait in _traits)
            trait.Tick();
    }

    public void OnSpawn()
    {
        foreach (ITrait trait in _traits)
            trait.OnSpawn();

        IsSpawned = true;
    }

    public void ApplyMoodlet<T>() where T : Moodlet
    {
        ApplyMoodlet<T>(-1);
    }

    public void ApplyMoodlet<T>(int expireOn) where T : Moodlet
    {
        Type type = typeof(T);

        Moodlet? existingMoodlet = _moodlets.FirstOrDefault(m => m.GetType() == type);
        if (existingMoodlet == null)
        {
            Moodlet m = _moodletFactory.CreateMoodlet<T>(expireOn);
            _moodlets.Add(m);
            m.Gain(this);
        }
        else if ((expireOn > existingMoodlet.ExpireOn && existingMoodlet.ExpireOn != -1) || expireOn == -1)
        {
            existingMoodlet.ExpireOn = expireOn;
        }
    }

    public bool HasMoodlet<T>() where T : Moodlet
    {
        Type type = typeof(T);
        return _moodlets.Any(m => m.GetType() == type);
    }

    public bool RemoveMoodlet<T>() where T : Moodlet
    {
        Type type = typeof(T);
        List<Moodlet> toRemove = _moodlets.Where(m => m.GetType() == type).ToList();
        if (toRemove is [])
            return false;

        _moodlets.RemoveAll(toRemove.Contains);
        foreach (Moodlet removedMoodlet in toRemove)
            removedMoodlet.OnLose();

        return true;
    }

    private void RemoveExpiredMoodlets()
    {
        List<Moodlet> toRemove = _moodlets
            .Where(m => m.ExpireOn != -1 && m.ExpireOn <= _world.CurrentTick)
            .ToList();

        if (toRemove is [])
            return;

        _moodlets.RemoveAll(toRemove.Contains);
        foreach (Moodlet removedMoodlet in toRemove)
        {
            removedMoodlet.OnExpire();
            removedMoodlet.OnLose();
        }
    }

    public void SetState<T>(T data) where T : struct, IState
    {
        Type type = typeof(T);
        _states[type] = data;
    }

    public T? GetState<T>() where T : struct, IState
    {
        Type type = typeof(T);

        T? result = (T?)_states.GetValueOrDefault(type);
        if (result == null)
            return null;

        return GetValueAfterModifiers(result.Value);
    }

    public T AddTrait<T>() where T : ITrait
    {
        if (_traits.OfType<T>().Any())
            throw new Exception($"The entity has trait '{typeof(T).Name}' already");

        T result = _traitFactory.CreateTrait<T>();
        _traits.Add(result);
        result.Gain(this);

        return result;
    }

    public bool TryAddTrait<T>() where T : ITrait
    {
        if (_traits.OfType<T>().Any())
            return false;

        T result = _traitFactory.CreateTrait<T>();
        _traits.Add(result);
        result.Gain(this);

        return true;
    }

    public T GetOrAddTrait<T>() where T : ITrait
    {
        T? result = _traits.OfType<T>().FirstOrDefault();

        if (result != null)
            return result; // Only 1 extension of given type allowed

        result = _traitFactory.CreateTrait<T>();
        _traits.Add(result);
        result.Gain(this);

        return result;
    }

    public bool HasTrait<T>() where T : ITrait
    {
        return _traits.Any(t => t is T);
    }

    public T GetTrait<T>() where T : ITrait
    {
        return TryGetTrait(out T? trait) ? trait : throw new Exception("Trait type does not exist on this entity");
    }

    public bool TryGetTrait<T>([NotNullWhen(true)] out T? trait) where T : ITrait
    {
        trait = (T?)_traits.FirstOrDefault(e => e is T);

        return trait != null;
    }

    public void AddModifier<T>(Func<T, T> modifier)
    {
        Type t = typeof(T);
        _modifiers.TryAdd(t, []);
        _modifiers[t].Add(modifier);
    }

    public void RemoveModifier<T>(Func<T, T> modifier)
    {
        Type t = typeof(T);
        _modifiers[t].Remove(modifier);
    }

    public T GetValueAfterModifiers<T>(T value)
    {
        Type t = typeof(T);

        List<object>? modifiers = _modifiers.GetValueOrDefault(t);
        if (modifiers is null or { Count: 0 })
            return value;

        foreach (Func<T, T> modifier in modifiers)
            value = modifier(value);

        return value;
    }
}
