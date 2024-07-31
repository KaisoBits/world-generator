using Microsoft.Extensions.Hosting;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace WorldGenerator;

public sealed class SfmlRenderer : IRenderer, IRendererVisitor<RenderStates>, IDisposable
{
    private readonly RenderWindow _window;

    private readonly Sprite _grass = new(new Texture("Resources/grass.png"));
    private readonly Sprite _castle = new(new Texture("Resources/village.png"));
    private readonly Sprite _dwarf = new(new Texture("Resources/dwarf.png"));
    private readonly Sprite _mountain = new(new Texture("Resources/mountain.png"));

    private readonly List<(IEntity Ent, RenderStates Rs)> _renderList = [];
    private readonly World _world;
    private readonly IHostApplicationLifetime _lifetime;

    public SfmlRenderer(World world, IHostApplicationLifetime lifetime)
    {
        _world = world;
        _lifetime = lifetime;
        _window = new(new VideoMode((uint)world.Width * 32, (uint)world.Height * 32), "World generator");
        _window.Closed += (s, e) => _lifetime.StopApplication();
        _window.Resized += (s, e) => _window.SetView(new View(new Vector2f(e.Width / 2, e.Height / 2), new Vector2f(e.Width, e.Height)));
    }

    public void Dispose()
    {
        _window.Dispose();
        _grass.Dispose();
        _castle.Dispose();
        _dwarf.Dispose();
        _mountain.Dispose();
    }

    public void Render()
    {
        _window.Clear(new Color(135, 206, 235));
        _window.DispatchEvents();

        foreach (ITileView tile in _world)
        {
            Transform t = Transform.Identity;
            t.Translate(new Vector2f(tile.Position.X * 32, tile.Position.Y * 32));
            RenderStates rs = new(t);

            _window.Draw(_grass, rs);

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

        _window.Display();
        _renderList.Clear();
    }

    public void VisitBuilding(IEntity building, RenderStates states)
    {
        _window.Draw(_castle, states);
    }

    public void VisitCreature(IEntity creature, RenderStates states)
    {
        _window.Draw(_dwarf, states);
    }

    public void VisitMountain(IEntity ground, RenderStates states)
    {
        _window.Draw(_mountain, states);
    }
}
