using WorldGenerator.States;

namespace WorldGenerator;

public class Building : Entity
{
    public override Layer Layer => Layer.Buildings;

    private readonly List<Creature> _population = [];

    private Building() { }

    public override void OnSpawn()
    {
        base.OnSpawn();

        EventBus.PublishEvent(new BuildingEstablishedEvent(this));
    }

    public void AddCitizen(Creature creature)
    {
        _population.Add(creature);
    }

    public override void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitBuilding(this, state);
    }

    public override void GatherConditions()
    {
        base.GatherConditions();

        int creatureCount = CurrentTile.Contents.Count(e => e is Creature);
        if (creatureCount > 0)
            ClearCondition<IsEmptyCondition>();
        else
            SetCondition<IsEmptyCondition>();
    }

    public static Building EstablishCity(string name)
    {
        Building building = new();
        building.SetState(new NameState(NameGenerator.GetFortressName()));

        return building;
    }
}
