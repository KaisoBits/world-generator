namespace WorldGenerator.Traits;

public class HealthTrait : Trait<HealthTrait.TraitData>
{
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    public void Damage(Damage damage)
    {
        Health = Math.Clamp(Health - damage.Amount, 0, MaxHealth);
    }

    public class TraitData
    {
        public int MaxHealth { get; init; }
    }
}

public enum DamageType { Generic }

public readonly struct Damage
{
    public required int Amount { get; init; }
    public required DamageType Type { get; init; }
    public required IEntity Attacker { get; init; }
}