namespace WorldGenerator;

public class BuildingEstablishedEvent : GameEvent
{
    public override EventType Type => EventType.BUILDING_ESTABLISHED;

    public override string? Message => "A building '{BUILDING_NAME}' has been established!";

    public BuildingEstablishedEvent(Building building)
    {
        SetParameter("BUILDING_NAME", building.Name);
    }
}
