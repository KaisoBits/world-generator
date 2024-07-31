namespace WorldGenerator.RenderActors;

public interface IRenderActor
{
    static virtual IRenderActor Instance => throw new NotImplementedException();

    void AcceptRenderer<T>(IEntity target, IRendererVisitor<T> renderer, T state);
}

public abstract class RenderActor<T> : IRenderActor where T : class, IRenderActor, new()
{
    protected RenderActor() { }

    public static IRenderActor Instance { get; } = new T();

    public abstract void AcceptRenderer<T1>(IEntity target, IRendererVisitor<T1> renderer, T1 state);

    public override string ToString() => GetType().Name;
}