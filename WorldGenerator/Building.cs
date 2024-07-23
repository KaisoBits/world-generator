using SFML.Graphics;

namespace WorldGenerator;

public class Building : IEntity
{
    public Layer Layer => Layer.Buildings;

    private readonly List<Creature> _population = [];
    private readonly List<Visitation> _vistations = [];

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
}
