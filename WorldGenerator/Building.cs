using SFML.Graphics;

namespace WorldGenerator;

public class Building : IEntity
{
    public Layer Layer => Layer.Buildings;

    public required string Name { get; init; }

    private readonly List<Creature> _population = [];
    private readonly List<Visitation> _vistations = [];

    private Building() { }

    public void AddCitizen(Creature creature)
    {
        _population.Add(creature);
        _vistations.Add(new Visitation(creature, this, VisitationPurpose.Citizen));
    }

    public void AddGuest(Creature creature)
    {
        _vistations.Add(new Visitation(creature, this, VisitationPurpose.Guest));
    }

    public void AcceptRenderer(IRenderer renderer, RenderStates renderStates)
    {
        renderer.AcceptBuilding(this, renderStates);
    }

    public static Building EstablishCity(string name)
    {
        Building building = new() { Name = name };

        EventBus.PublishEvent(new BuildingEstablishedEvent(building));

        return building;
    }
}
