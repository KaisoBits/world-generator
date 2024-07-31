using WorldGenerator.States;

namespace WorldGenerator.Moodlets;

public class InBuildingMoodlet : Moodlet
{
    private IEntity? _owner;

    public override int MoodModifier => 10;
    public override string Name => "Like at home";
    public override string Description => $"Any dwarf prefers cold walls of a brick building to the dizzying rays of sunshine. " +
        $"{_owner?.GetState<NameState>()?.Name} is glad to be in a building";

    public override void OnAquire(IEntity creature)
    {
        _owner = creature;
    }
}
