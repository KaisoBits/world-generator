namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public IEntity? Owner { get; private set; }

    public virtual string Name => GetType().Name;
    public virtual string Description => string.Empty;

    public int ExpireOn { get; set; }

    public virtual void Acquire(IEntity owner) 
    {
        Owner = owner;
        OnAcquire();
    }

    protected virtual void OnAcquire() { }
    public virtual void OnExpire() { }
    public virtual void OnLost() { }

    public virtual void Tick() { }
}
