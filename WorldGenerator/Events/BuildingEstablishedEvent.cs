using WorldGenerator.States;

namespace WorldGenerator.Events;

public class BuildingEstablishedEvent : GameEvent
{
    public override string? Message => $"A building '{BuildingName}' has been established!";

    public string? BuildingName { get; }

    public BuildingEstablishedEvent(IEntity building)
    {
        BuildingName = building.GetState<NameState>()?.Name;
    }
}
