using SFML.Graphics;
using SFML.System;

namespace WorldGenerator;

public class SfmlRenderer : IRenderer, IRendererVisitor<RenderStates>
{
    private readonly RenderTarget _target;

    private readonly Sprite _grass = new(new Texture("grass.png"));
    private readonly Sprite _castle = new(new Texture("village.png"));
    private readonly Sprite _dwarf = new(new Texture("dwarf.png"));

    private readonly List<(IEntity Ent, RenderStates Rs)> _renderList = [];

    public SfmlRenderer(RenderTarget target)
    {
        _target = target;
    }

    public void RenderWorld(World world)
    {
        foreach (ITileView tile in world)
        {
            Transform t = Transform.Identity;
            t.Translate(new Vector2f(tile.Position.X * 32, tile.Position.Y * 32));
            RenderStates rs = new(t);

            _target.Draw(_grass, rs);

            foreach (IEntity entity in tile.Contents)
            {
                _renderList.Add((entity, rs));
            }
        }

        // TODO: Inplace sorting
        foreach (var (entity, rs) in _renderList
            .OrderBy(e => e.Ent.Layer)
            .ThenBy(e => e.Ent.Position.Y)
            .ThenBy(e => e.Ent.Position.X))
        {
            entity.AcceptRenderer(this, rs);
        }

        _renderList.Clear();
    }

    public void VisitBuilding(Building building, RenderStates states)
    {
        _target.Draw(_castle, states);
    }

    public void VisitCreature(Creature creature, RenderStates states)
    {
        _target.Draw(_dwarf, states);
    }
}
