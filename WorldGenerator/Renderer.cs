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

    public void RenderTiles(Tile[,] tiles)
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Transform t = Transform.Identity;
                t.Translate(new Vector2f(x * 32, y * 32));
                RenderStates rs = new(t);

                _target.Draw(_grass, rs);

                IEnumerable<IEntity> currTileContent = tiles[x, y].Contents;
                foreach (IEntity entity in currTileContent)
                {
                    entity.AcceptRenderer(this, rs);
                }
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
