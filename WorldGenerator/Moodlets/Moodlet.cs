namespace WorldGenerator.Moodlets;

public abstract class Moodlet
{
    public abstract int MoodModifier { get; }

    public abstract string Name { get; }
    public abstract string Description { get; }

    public int ExpireOn { get; set; }

    public virtual void OnAquire(Creature creature) { }
    public virtual void OnExpire(Creature creature) { }
    public virtual void OnLost(Creature creature) { }
}
