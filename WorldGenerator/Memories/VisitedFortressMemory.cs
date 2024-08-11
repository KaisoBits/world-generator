namespace WorldGenerator.Memories;

public class VisitedFortressMemory : IEntityMemory
{
    private readonly string _buildingName;

    public string Message => $"Visited {_buildingName}";

    public VisitedFortressMemory(string buildingName)
    {
        _buildingName = buildingName;
    }
}
