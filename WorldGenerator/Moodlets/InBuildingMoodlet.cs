using WorldGenerator.States;

namespace WorldGenerator.Moodlets;

public class InBuildingMoodlet : Moodlet
{
    public override int MoodModifier => 10;
    public override string Name => "Like at home";
    public override string Description => $"Any dwarf prefers cold walls of a brick building to the dizzying rays of sunshine. " +
        $"{OwnerEntity?.GetState<NameState>()?.Name} is glad to be in a building";
}
