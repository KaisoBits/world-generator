namespace WorldGenerator.EntityExtensions;

public interface IEntityExtension
{
    IEntity? Owner { get; }

    void Tick();

    void OnGatherConditions();
    void OnSpawn();
    void OnLose();

    void Gain(IEntity owner);
}
