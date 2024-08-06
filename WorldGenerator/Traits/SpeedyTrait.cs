using WorldGenerator.States;

namespace WorldGenerator.Traits;

public class SpeedyTrait : Trait<NullTraitData>
{
    protected override void OnGain()
    {
        Owner.RegisterModifier<SpeedState>(SpeedModifier);
    }

    public override void OnLose()
    {
        Owner.DeregisterModifier<SpeedState>(SpeedModifier);
    }

    private SpeedState SpeedModifier(SpeedState s) => new(s.Speed * 2);
}
