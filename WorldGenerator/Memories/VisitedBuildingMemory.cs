namespace WorldGenerator.Memories;

public class VisitedBuildingMemory : CreatureMemory
{
    private readonly string _buildingName;

    public override string Message => $"Visited {_buildingName}";

    public VisitedBuildingMemory(string buildingName)
    {
        _buildingName = buildingName;
    }
}
