using WorldGenerator.Factories;
using WorldGenerator.Moodlets;

namespace WorldGenerator.EntityExtensions;

public class MoodExtension : EntityExtension
{
    public int MoodLevel
    {
        get
        {
            if (_moodRecalcRequired)
                RecalcMood();

            return _moodLevel;
        }
    }
    private int _moodLevel;

    public IReadOnlyList<Moodlet> Moodlets => _moodlets;
    private readonly List<Moodlet> _moodlets = [];

    private readonly World _world;
    private readonly MoodletFactory _moodletFactory;

    private bool _moodRecalcRequired = true;

    public MoodExtension(World world, MoodletFactory moodletFactory)
    {
        _world = world;
        _moodletFactory = moodletFactory;
    }

    public void ApplyMoodlet<T>(int expireOn) where T : Moodlet
    {
        Type type = typeof(T);

        Moodlet? existingMoodlet = _moodlets.FirstOrDefault(m => m.GetType() == type);
        if (existingMoodlet == null)
        {
            Moodlet m = _moodletFactory.CreateMoodlet<T>(expireOn);
            _moodlets.Add(m);
            m.Acquire(this);
            ScheduleMoodRecalc();
        }
        else if ((expireOn > existingMoodlet.ExpireOn && existingMoodlet.ExpireOn != -1) || expireOn == -1)
        {
            existingMoodlet.ExpireOn = expireOn;
        }
    }

    public bool HasMoodlet<T>() where T : Moodlet
    {
        Type type = typeof(T);
        return _moodlets.Any(m => m.GetType() == type);
    }

    public bool RemoveMoodlet<T>() where T : Moodlet
    {
        Type type = typeof(T);
        List<Moodlet> toRemove = _moodlets.Where(m => m.GetType() == type).ToList();
        if (toRemove is [])
            return false;

        _moodlets.RemoveAll(toRemove.Contains);
        foreach (Moodlet removedMoodlet in toRemove)
            removedMoodlet.OnLost();

        ScheduleMoodRecalc();

        return true;
    }

    private void RemoveExpiredMoodlets()
    {
        List<Moodlet> toRemove = _moodlets.Where(m => m.ExpireOn != -1 && m.ExpireOn <= _world.CurrentTick).ToList();
        if (toRemove is [])
            return;

        _moodlets.RemoveAll(toRemove.Contains);
        foreach (Moodlet removedMoodlet in toRemove)
        {
            removedMoodlet.OnExpire();
            removedMoodlet.OnLost();
        }

        ScheduleMoodRecalc();
    }

    public override void Tick()
    {
        RemoveExpiredMoodlets();
    }

    private void ScheduleMoodRecalc()
    {
        _moodRecalcRequired = true;
    }

    private void RecalcMood()
    {
        _moodLevel = _moodlets.Sum(m => m.MoodModifier);
        _moodRecalcRequired = false;
    }
}
