using WorldGenerator.States;

namespace WorldGenerator;

public class BuildingEstablishedEvent : GameEvent
{
    public override EventType Type => EventType.BUILDING_ESTABLISHED;

    public override string? Message => "A building '{BUILDING_NAME}' has been established!";

    public BuildingEstablishedEvent(IEntity building)
    {
        SetParameter("BUILDING_NAME", building.GetState<NameState>()?.Name);
    }
}
