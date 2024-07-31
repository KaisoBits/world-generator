namespace WorldGenerator.Traits;

public abstract class Trait<TData> : ITrait
{
    public IEntity Owner { get; private set; } = default!;
    public TData TraitData { get; }

    protected Trait(TData data)
    {
        TraitData = data;
    }

    public void Gain(IEntity owner) 
    {
        Owner = owner;

        OnGain();
    }
    protected virtual void OnGain() { }

    public virtual void OnSpawn() { }

    public virtual void OnLose() { }

    public virtual void Tick() { }

    public virtual void OnGatherConditions() { }
}
public class NullTraitData;