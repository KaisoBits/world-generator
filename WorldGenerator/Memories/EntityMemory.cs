namespace WorldGenerator.Memories;

public abstract class EntityMemory
{
    public abstract string Message { get; }

    public override string ToString() => Message;
}
