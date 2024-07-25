namespace WorldGenerator;

public interface IRendererVisitor<T>
{
    void VisitBuilding(Building building, T state);
    void VisitCreature(Creature creature, T state);
}
