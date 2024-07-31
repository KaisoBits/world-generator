namespace WorldGenerator.Memories;

public class VisitedBuildingMemory : EntityMemory
{
    private readonly string _buildingName;

    public override string Message => $"Visited {_buildingName}";

    public VisitedBuildingMemory(string buildingName)
    {
        _buildingName = buildingName;
    }
}
