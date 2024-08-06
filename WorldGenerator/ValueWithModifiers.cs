namespace WorldGenerator;

public interface IValueWithModifiers;

public sealed class ValueWithModifiers<T> : IValueWithModifiers
{
    // TODO: Cache it?
    public T Value
    {
        get
        {
            T result = OriginalValue;
            foreach (var modifier in _modifiers)
                result = modifier(result);

            return result;
        }
    }

    public T OriginalValue { get; set; }
    private readonly List<Func<T, T>> _modifiers = [];

    public ValueWithModifiers(T initialValue)
    {
        OriginalValue = initialValue;
    }

    public void RegisterModifier(Func<T, T> modifier)
    {
        _modifiers.Add(modifier);
    }

    // TODO: Subscription system similar to what EventBus has?
    public void DeregisterModifier(Func<T, T> modifier)
    {
        _modifiers.Remove(modifier);
    }

    public override string ToString()
    {
        return $"{Value} ({OriginalValue})";
    }
}
