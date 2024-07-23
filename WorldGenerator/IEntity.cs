using SFML.Graphics;

namespace WorldGenerator;

public interface IEntity
{
    Layer Layer { get; }

    void AcceptRenderer(IRenderer renderer, RenderStates states);
}
