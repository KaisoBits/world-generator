namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public abstract int MoodModifier { get; }

    public abstract string Name { get; }
    public abstract string Description { get; }

    public int ExpireOn { get; set; }

    public virtual void OnAquire(IEntity creature) { }
    public virtual void OnExpire(IEntity creature) { }
    public virtual void OnLost(IEntity creature) { }
}
