namespace WorldGenerator.Moodlets;

public class InBuildingMoodlet : Moodlet
{
    private Creature? _owner;

    public override int MoodModifier => 10;
    public override string Name => "Like at home";
    public override string Description => $"Any dwarf prefers cold walls of a brick building to the dizzying rays of sunshine. " +
        $"{_owner?.GetState(State.Name)} is glad to be in a building";

    public override void OnAquire(Creature creature)
    {
        _owner = creature;
    }
}
