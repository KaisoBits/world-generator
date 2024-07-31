namespace WorldGenerator.RenderActors;

public class MountainRenderActor : RenderActor<MountainRenderActor>
{
    public override void AcceptRenderer<T>(IEntity target, IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitMountain(target, state);
    }
}
