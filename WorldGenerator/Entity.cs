﻿using SFML.Graphics;

namespace WorldGenerator;

public abstract class Entity : IEntity
{
    private readonly HashSet<Condition> _conditions = [];
    private readonly Dictionary<State, string> _states = [];
    public abstract Layer Layer { get; }

    public abstract void AcceptRenderer(IRenderer renderer, RenderStates states);

    protected virtual void GatherConditions()
    {
    }

    public virtual void OnSpawn() { }

    public bool IsInCondition(Condition condition)
    {
        return _conditions.Contains(condition);
    }

    public void SetCondition(Condition condition)
    { 
        _conditions.Add(condition);
    }

    public void ClearCondition(Condition condition)
    {
        _conditions.Remove(condition);
    }

    public void SetState(State state, string value)
    {
        _states[state] = value;
    }

    public string? GetState(State state)
    {
        return _states.GetValueOrDefault(state);
    }

    public int GetStateInt(State state)
    {
        return int.Parse(_states[state]);
    }

    public float GetStateFloat(State state)
    {
        return float.Parse(_states[state]);
    }
}
