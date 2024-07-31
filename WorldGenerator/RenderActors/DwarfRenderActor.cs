namespace WorldGenerator.RenderActors;

public class DwarfRenderActor : RenderActor<DwarfRenderActor>
{
    public override void AcceptRenderer<T>(IEntity target, IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitCreature(target, state);
    }
}
