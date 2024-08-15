using System.Diagnostics.CodeAnalysis;

namespace WorldGenerator.Traits;

public abstract class Trait<TData> : ITrait where TData : new()
{
    public IEntity Owner { get; private set; } = default!;

    protected TData Data { get; private set; } = new();

    public virtual bool Hidden => false;
    public virtual bool CanBeRemoved => false;

    public virtual string Name => GetType().Name;

    public virtual string Description => string.Empty;

    public void Gain(IEntity owner)
    {
        Owner = owner;

        OnGain();
    }

    protected virtual void OnGain() { }

    public virtual void OnSpawn() { }

    public virtual void OnLose() { }

    public virtual void Tick() { }

    public void WithData(TData data) => Data = data;

    protected T RequireTrait<T>() where T : ITrait
    {
        if (Owner == null)
            throw new Exception($"The trait '{GetType().Name}' cannot require trait '{typeof(T).Name}' because '{nameof(Owner)}' is not set");

        if (!Owner.TryGetTrait<T>(out T? result))
            throw new Exception($"The trait '{GetType().Name}' requires trait '{typeof(T).Name}' to be set first, in order to work correctly");

        return result;
    }

    protected bool TryGetTrait<T>([NotNullWhen(true)] out T? trait) where T : ITrait
    {
        if (Owner == null || !Owner.TryGetTrait<T>(out T? result))
        {
            trait = default;
            return false;
        }

        trait = result;

        return true;
    }
}
public class NullTraitData;