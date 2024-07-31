using WorldGenerator.EntityExtensions;

namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public MoodExtension? Owner { get; private set; }
    public IEntity? OwnerEntity => Owner?.Owner;

    public abstract int MoodModifier { get; }

    public abstract string Name { get; }
    public abstract string Description { get; }

    public int ExpireOn { get; set; }

    public virtual void Acquire(MoodExtension mood) 
    {
        Owner = mood;
        OnAcquire();
    }

    protected virtual void OnAcquire() { }
    public virtual void OnExpire() { }
    public virtual void OnLost() { }
}
