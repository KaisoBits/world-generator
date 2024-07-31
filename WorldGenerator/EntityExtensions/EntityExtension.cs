
namespace WorldGenerator.EntityExtensions;

public class EntityExtension : IEntityExtension
{
    public IEntity Owner { get; private set; } = default!;

    public void Gain(IEntity owner)
    {
        Owner = owner;

        OnGain();
    }

    protected virtual void OnGain() { }

    public virtual void OnLose() { }

    public virtual void OnSpawn() { }

    public virtual void Tick() { }

    public virtual void OnGatherConditions() { }
}
