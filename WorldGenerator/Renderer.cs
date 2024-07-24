using SFML.Graphics;
using SFML.System;

namespace WorldGenerator;

public class Renderer : IRenderer
{
    private readonly RenderTarget _target;

    private readonly Sprite _grass = new(new Texture("grass.png"));
    private readonly Sprite _castle = new(new Texture("castle.png"));

    public Renderer(RenderTarget target)
    {
        _target = target;
        _castle.Scale = new Vector2f(0.055f, 0.055f);
    }

    public void RenderWorld(World world)
    {
        foreach (ITileView tile in world)
        {
            Transform t = Transform.Identity;
            t.Translate(new Vector2f(tile.X * 32, tile.Y * 32));
            RenderStates rs = new(t);

            _target.Draw(_grass, rs);

            foreach (IEntity entity in tile.Contents)
            {
                entity.AcceptRenderer(this, rs);
            }
        }
    }

    public void AcceptBuilding(Building building, RenderStates renderStates)
    {
        _target.Draw(_castle, renderStates);
    }

    public void AcceptCreature(Creature creature, RenderStates states)
    {
        throw new NotImplementedException();
    }
}
