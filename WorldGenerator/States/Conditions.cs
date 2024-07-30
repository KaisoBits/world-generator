namespace WorldGenerator.States;

public class IsEmptyCondition : Condition<IsEmptyCondition>;
public class InBuildingCondition : Condition<InBuildingCondition>;
public class JustEnteredBuildingCondition : Condition<JustEnteredBuildingCondition>;
public class DeadCondition : Condition<DeadCondition>;

public interface ICondition
{
    static virtual ICondition Instance => throw new NotImplementedException();
}

public abstract class Condition<T> : ICondition where T : class, ICondition, new()
{
    public static ICondition Instance { get; } = new T();

    public override string ToString() => GetType().Name;
}