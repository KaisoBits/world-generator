namespace WorldGenerator;

public class EventBus
{
    public static IReadOnlyList<GameEvent> EventList => _events;
    private static readonly List<GameEvent> _events = [];

    private static int _currSubIndex = 1;
    private static readonly List<Subscription> _subscribers = [];

    public void PublishEvent(GameEvent ev)
    {
        _events.Add(ev);

        foreach (Subscription sub in _subscribers.Where(s => ev.GetType().IsAssignableTo(s.Type)))
            sub.Callback(ev);
    }

    public Subscription Subscribe<T>(Action<T> callback) where T : GameEvent
    {
        Subscription sub = new(_currSubIndex++, typeof(T), (GameEvent ev) => callback((T)ev));

        _subscribers.Add(sub);

        return sub;
    }

    public void Unsubscribe(Subscription subscription)
    {
        _subscribers.Remove(subscription);
    }
}

public readonly struct Subscription : IEquatable<Subscription>
{
    public int Identifier { get; }
    public Type Type { get; }
    public Action<GameEvent> Callback { get; }

    public Subscription(int identifier, Type type, Action<GameEvent> callback)
    {
        Identifier = identifier;
        Type = type;
        Callback = callback;
    }

    public void Unsubscribe()
    {
        //EventBus.Unsubscribe(this);
    }

    public override bool Equals(object? obj) => obj is Subscription other && Equals(other);

    public bool Equals(Subscription sub) => Identifier == sub.Identifier;

    public override int GetHashCode() => Identifier.GetHashCode();

    public static bool operator ==(Subscription lhs, Subscription rhs) => lhs.Equals(rhs);

    public static bool operator !=(Subscription lhs, Subscription rhs) => !(lhs == rhs);
};