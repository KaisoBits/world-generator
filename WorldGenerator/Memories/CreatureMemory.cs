namespace WorldGenerator.Memories;

public abstract class CreatureMemory
{
    public abstract string Message { get; }

    public override string ToString() => Message;
}
