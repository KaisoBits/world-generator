namespace WorldGenerator.Traits;

public class Need
{
    public float Value { get; set; }
    public ValueWithModifiers<float> Min { get; } = new(0);
    public ValueWithModifiers<float> Max { get; } = new(0);
    public ValueWithModifiers<float> ChangeRate { get; } = new(0);

    public Need(float min, float max, float changeRate)
    {
        Min.OriginalValue = min;
        Max.OriginalValue = max;
        ChangeRate.OriginalValue = changeRate;
    }
}

public class NeedsTrait : Trait<NeedsTrait>
{
    private readonly Dictionary<string, Need> _needs = [];

    public void SetNeed(string needName, Need need)
    {
        _needs[needName] = need;
    }

    public Need GetNeed(string needName)
    {
        return _needs[needName];
    }

    public override void Tick()
    {
        foreach (var (_, need) in _needs)
        {
            need.Value = Math.Clamp(need.Value + need.ChangeRate.Value, need.Min.Value, need.Max.Value);
        }
    }
}
