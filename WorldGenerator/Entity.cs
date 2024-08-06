using System.Diagnostics.CodeAnalysis;
using WorldGenerator.Factories;
using WorldGenerator.States;
using WorldGenerator.Traits;

namespace WorldGenerator;

public sealed class Entity : IEntity
{
    public ISet<ICondition> Conditions => _conditions;
    private readonly HashSet<ICondition> _conditions = [];

    public IReadOnlyCollection<IState> States => _states.Values;
    private readonly Dictionary<Type, IState> _states = [];

    public IReadOnlyCollection<ITrait> Traits => _traits;
    private readonly List<ITrait> _traits = [];

    public string? RenderType { get; set; }

    public Vector Position { get; internal set; }

    public ITileView CurrentTile => _world[Position];

    public Layer Layer { get; set; }
    public bool IsSpawned { get; private set; }

    private readonly World _world;
    private readonly TraitFactory _traitFactory;

    public Entity(World world, TraitFactory traitFactory)
    {
        _world = world;
        _traitFactory = traitFactory;
    }

    public void Think()
    {
        foreach (ITrait trait in _traits)
            trait.Tick();
    }

    public void GatherConditions()
    {
        foreach (ITrait trait in _traits)
            trait.OnGatherConditions();
    }

    public void OnSpawn()
    {
        foreach (ITrait trait in _traits)
            trait.OnSpawn();

        IsSpawned = true;
    }

    public bool InCondition<T>() where T : ICondition
    {
        return _conditions.Contains(T.Instance);
    }

    public void SetCondition<T>() where T : ICondition
    {
        _conditions.Add(T.Instance);
    }

    public bool ClearCondition<T>() where T : ICondition
    {
        return _conditions.Remove(T.Instance);
    }

    public void SetState(IState state)
    {
        _states[state.GetType()] = state;
    }

    public T? GetState<T>() where T : class, IState
    {
        return _states.GetValueOrDefault(typeof(T)) as T;
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
}
