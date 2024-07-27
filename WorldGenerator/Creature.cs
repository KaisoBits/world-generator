using WorldGenerator.AI;
using WorldGenerator.Memories;
using WorldGenerator.Moodlets;

namespace WorldGenerator;

public class Creature : Entity
{
    public override Layer Layer => Layer.Creatures;

    public int MoodLevel { get; private set; }

    private bool _moodRecalcRequired = true;

    public IReadOnlyCollection<Moodlet> Moodlets => _moodlets;
    private readonly List<Moodlet> _moodlets = [];

    public IReadOnlyCollection<CreatureMemory> Memories => _memories;
    private readonly List<CreatureMemory> _memories = [];

    public override void OnSpawn()
    {
        base.OnSpawn();
        AddBehaviour(new CitizenBehaviour());

        SetState(State.Health, "100");
    }

    public override void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitCreature(this, state);
    }

    public override void GatherConditions()
    {
        base.GatherConditions();

        int health = GetStateInt(State.Health);
        if (health <= 0)
            SetCondition(Condition.DEAD);
        else
            ClearCondition(Condition.DEAD);

        ClearCondition(Condition.JUST_ENTERED_BUILDING);

        if (CurrentTile.Contents.Any(e => e.Layer == Layer.Buildings))
        {
            if (!InCondition(Condition.IN_BUILDING))
            {
                SetCondition(Condition.IN_BUILDING);
                SetCondition(Condition.JUST_ENTERED_BUILDING);
            }
        }
        else
        {
            ClearCondition(Condition.IN_BUILDING);
        }
    }

    public override void Think()
    {
        base.Think();

        if (InCondition(Condition.IN_BUILDING))
        {
            ApplyMoodlet<InBuildingMoodlet>(World.Instance.CurrentTick + 5);
        }

        if (InCondition(Condition.JUST_ENTERED_BUILDING))
        {
            IEntity? building = CurrentTile.Contents.FirstOrDefault(e => e is Building);
            if (building != null)
                Remember(new VisitedBuildingMemory(building.GetState(State.Name) ?? string.Empty));
        }

        RemoveExpiredMoodlets();

        if (_moodRecalcRequired)
            RecalcMood();
    }

    public void ApplyMoodlet<T>(int expireOn) where T : Moodlet, new()
    {
        Type type = typeof(T);

        Moodlet? existingMoodlet = _moodlets.FirstOrDefault(m => m.GetType() == type);
        if (existingMoodlet == null)
        {
            Moodlet m = new T { ExpireOn = expireOn };
            _moodlets.Add(m);
            m.OnAquire(this);
            ScheduleMoodRecalc();
        }
        else if (expireOn > existingMoodlet.ExpireOn || expireOn == -1)
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
            removedMoodlet.OnLost(this);

        ScheduleMoodRecalc();

        return true;
    }

    private void RemoveExpiredMoodlets()
    {
        List<Moodlet> toRemove = _moodlets.Where(m => m.ExpireOn != -1 && m.ExpireOn <= World.Instance.CurrentTick).ToList();
        if (toRemove is [])
            return;

        _moodlets.RemoveAll(toRemove.Contains);
        foreach (Moodlet removedMoodlet in toRemove)
        {
            removedMoodlet.OnExpire(this);
            removedMoodlet.OnLost(this);
        }

        ScheduleMoodRecalc();
    }

    private void ScheduleMoodRecalc()
    {
        _moodRecalcRequired = true;
    }

    private void RecalcMood()
    {
        MoodLevel = _moodlets.Sum(m => m.MoodModifier);
        _moodRecalcRequired = false;
    }

    public void Remember(CreatureMemory memory)
    {
        _memories.Add(memory);
    }
}
