using SFML.Graphics;

namespace WorldGenerator;

public class Creature : Entity
{
    public override Layer Layer => Layer.Creatures;

    public override void OnSpawn()
    {
        base.OnSpawn();

        SetState(State.Health, "100");
    }

    public override void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitCreature(this, state);
    }

    public override void GatherConditions()
    {
        base.GatherConditions();

        int health = GetStateInt(State.Health);
        if (health <= 0)
            SetCondition(Condition.DEAD);
        else
            ClearCondition(Condition.DEAD);
    }
}
