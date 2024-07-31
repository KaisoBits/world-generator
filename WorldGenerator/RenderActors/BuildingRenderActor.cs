namespace WorldGenerator.RenderActors;

public class BuildingRenderActor : RenderActor<BuildingRenderActor>
{
    public override void AcceptRenderer<T>(IEntity target, IRendererVisitor<T> renderer, T state)
    {
        renderer.VisitBuilding(target, state);
    }
}
