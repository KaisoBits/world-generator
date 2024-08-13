namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public IEntity? Owner { get; private set; }

    public virtual string Name => GetType().Name;
    public virtual string Description => string.Empty;

    public int ExpireOn { get; set; }

    public virtual void Gain(IEntity owner) 
    {
        Owner = owner;
        OnGain();
    }

    protected virtual void OnGain() { }
    public virtual void OnExpire() { }
    public virtual void OnLose() { }

    public virtual void Tick() { }
}
