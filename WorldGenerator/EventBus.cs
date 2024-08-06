using WorldGenerator.Events;

namespace WorldGenerator;

public class EventBus
{
    public IReadOnlyList<GameEvent> EventList => _events;
    private readonly List<GameEvent> _events = [];

    private int _currSubIndex = 1;
    private readonly List<Subscription> _subscribers = [];

    public void PublishEvent(GameEvent ev)
    {
        _events.Add(ev);

        foreach (Subscription sub in _subscribers.Where(s => ev.GetType().IsAssignableTo(s.Type)))
            sub.Callback(ev);
    }

    public Subscription Subscribe<T>(Action<T> callback) where T : GameEvent
    {
        Subscription sub = new(this, _currSubIndex++, typeof(T), (GameEvent ev) => callback((T)ev));

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
    private readonly EventBus _eventBus;
    private readonly int _identifier;

    public Type Type { get; }
    public Action<GameEvent> Callback { get; }

    public Subscription(EventBus eventBus, int identifier, Type type, Action<GameEvent> callback)
    {
        _eventBus = eventBus;
        _identifier = identifier;
        Type = type;
        Callback = callback;
    }

    public void Unsubscribe()
    {
        _eventBus.Unsubscribe(this);
    }

    public override bool Equals(object? obj) => obj is Subscription other && Equals(other);

    public bool Equals(Subscription sub) => _identifier == sub._identifier;

    public override int GetHashCode() => _identifier.GetHashCode();

    public static bool operator ==(Subscription lhs, Subscription rhs) => lhs.Equals(rhs);

    public static bool operator !=(Subscription lhs, Subscription rhs) => !(lhs == rhs);
};