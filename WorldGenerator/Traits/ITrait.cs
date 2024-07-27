namespace WorldGenerator.Traits;

public interface ITrait
{
    IEntity Owner { get; }

    void Tick();
    void OnGain(IEntity owner);
    void OnLose();
}
