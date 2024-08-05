using WorldGenerator.Traits;

namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public MoodTrait? Owner { get; private set; }
    public IEntity? OwnerEntity => Owner?.Owner;

    public abstract int MoodModifier { get; }

    public abstract string Name { get; }
    public abstract string Description { get; }

    public int ExpireOn { get; set; }

    public virtual void Acquire(MoodTrait owner) 
    {
        Owner = owner;
        OnAcquire();
    }

    protected virtual void OnAcquire() { }
    public virtual void OnExpire() { }
    public virtual void OnLost() { }
}
