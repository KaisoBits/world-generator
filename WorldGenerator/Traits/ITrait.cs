namespace WorldGenerator.Traits;

public interface ITrait
{
    IEntity? Owner { get; }

    bool Hidden { get; }
    bool CanBeRemoved { get; }

    string Name { get; }
    string Description { get; }

    void Tick();

    void OnSpawn();

    void Gain(IEntity owner);
    void OnLose();
}
