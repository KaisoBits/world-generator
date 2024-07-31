
namespace WorldGenerator;

public interface IRendererVisitor<T>
{
    void VisitBuilding(IEntity building, T state);
    void VisitCreature(IEntity creature, T state);
    void VisitMountain(IEntity ground, T state);
}
