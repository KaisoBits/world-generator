using WorldGenerator.Factories;

namespace WorldGenerator.Traits;

public class MoodTrait : Trait<NullTraitData>
{
    public int MoodLevel => _moodLevel;
    private int _moodLevel;

    private readonly World _world;
    private readonly MoodletFactory _moodletFactory;

    private bool _moodRecalcRequired = true;

    public MoodTrait(World world, MoodletFactory moodletFactory)
    {
        _world = world;
        _moodletFactory = moodletFactory;
    }

    private void ScheduleMoodRecalc()
    {
        _moodRecalcRequired = true;
    }

}

