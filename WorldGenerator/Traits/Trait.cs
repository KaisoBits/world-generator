namespace WorldGenerator.Traits;

public abstract class Trait<TData> : ITrait
{
    public IEntity? Owner { get; private set; }
    public TData TraitData { get; }

    protected Trait(TData data)
    {
        TraitData = data;
    }

    public virtual void OnGain(IEntity owner) 
    {
        Owner = owner;
    }

    public virtual void OnLose() { }

    public virtual void Tick() { }
}
public class NullTraitData;