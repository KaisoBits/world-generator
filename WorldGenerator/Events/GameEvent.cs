namespace WorldGenerator.Events;

public abstract partial class GameEvent
{
    public virtual string? Message { get; }

    public override string ToString()
    {
        if (Message == null)
            return $"[{GetType().Name}]";

        return $"[{GetType().Name}] {Message}";
    }
}
