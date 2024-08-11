using WorldGenerator.States;

namespace WorldGenerator.Traits;

public class SpeedyTrait : Trait<NullTraitData>
{
    protected override void OnGain()
    {
        Owner.AddModifier<SpeedState>(SpeedModifier);
    }

    public override void OnLose()
    {
        Owner.RemoveModifier<SpeedState>(SpeedModifier);
    }

    private SpeedState SpeedModifier(SpeedState s) => new(s.Speed * 2);
}
