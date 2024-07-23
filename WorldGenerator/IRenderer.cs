using SFML.Graphics;

namespace WorldGenerator;

public interface IRenderer
{
    void AcceptBuilding(Building building, RenderStates states);
    void AcceptCreature(Creature creature, RenderStates states);
}
