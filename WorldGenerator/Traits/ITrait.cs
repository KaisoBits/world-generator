namespace WorldGenerator.Traits;

public interface ITrait
{
    IEntity? Owner { get; }

    void Tick();

    void OnGatherConditions();
    void OnSpawn();

    void Gain(IEntity owner);
    void OnLose();
}
