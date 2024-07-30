namespace WorldGenerator.States;

public record class IsEmptyCondition : Condition<IsEmptyCondition>;
public record class InBuildingCondition : Condition<InBuildingCondition>;
public record class JustEnteredBuildingCondition : Condition<JustEnteredBuildingCondition>;
public record class DeadCondition : Condition<DeadCondition>;

public interface ICondition
{
    static virtual ICondition Instance => throw new NotImplementedException();
}

public record class Condition<T> : ICondition where T : class, ICondition, new()
{
    public static ICondition Instance { get; } = new T();

    public override string ToString() => GetType().Name;
}