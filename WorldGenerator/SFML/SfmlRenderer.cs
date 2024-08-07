using Microsoft.Extensions.Hosting;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator.States;

namespace WorldGenerator.SFML;

public sealed class SFMLRenderer : IRenderer, IDisposable
{
    private readonly RenderWindow _window;
    private readonly View _view;

    private readonly Sprite _grass = new(LoadTexture("Resources/grass.png"));
    private readonly Sprite _castle = new(LoadTexture("Resources/village.png"));
    private readonly Sprite _dwarf = new(LoadTexture("Resources/dwarf.png"));
    private readonly Sprite _mountain = new(LoadTexture("Resources/mountain.png"));
    private readonly Sprite _smallMountain = new(LoadTexture("Resources/stone.png"));
    private readonly Sprite _field = new(LoadTexture("Resources/field.png"));

    private readonly Shape _highlight = new RectangleShape(new Vector2f(32, 32))
    {
        FillColor = Color.Transparent,
        OutlineColor = Color.Red,
        OutlineThickness = -3,
    };

    private readonly Dictionary<string, Action<IEntity, RenderStates>> _renders = [];

    private readonly List<(IEntity Ent, RenderStates Rs)> _renderList = [];
    private readonly World _world;
    private readonly DebugOverlay _debug;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ConsoleInterface _consoleInterface;
    bool _isMovingCam = false;
    Vector2f _lastPos = new();
    float _zoom = 1.0f;

    ITileView? _selectedTile;
    ITileView? _lastSelectedTile;
    int _lastTick = -1;

    public SFMLRenderer(World world, DebugOverlay overlay, IHostApplicationLifetime lifetime, EventBus eventBus, ConsoleInterface consoleInterface)
    {
        _world = world;
        _debug = overlay;
        _lifetime = lifetime;
        _consoleInterface = consoleInterface;

        ContextSettings settings = new()
        {
            AntialiasingLevel = 4
        };
        _window = new(new VideoMode(Math.Min((uint)world.Width * 32, 1280), Math.Min((uint)world.Height * 32, 720)), "World generator", Styles.Default, settings);
        _view = new View(new Vector2f(_window.Size.X / 2, _window.Size.Y / 2), new Vector2f(_window.Size.X, _window.Size.Y));
        _window.SetView(_view);

        _window.Closed += (s, e) => _lifetime.StopApplication();
        _window.Resized += (s, e) => _view.Size = new Vector2f(e.Width, e.Height) * _zoom;
        _window.MouseWheelScrolled += (s, e) =>
        {
            float multiplier = Math.Abs(e.Delta);
            float ratio = (e.Delta < 0 ? 1.25f * multiplier : 0.8f / multiplier);
            _zoom = Math.Clamp(_zoom * ratio, 0.1f, 3.0f);

            _view.Size = (Vector2f)_window.Size * _zoom;
        };

        _window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button == Mouse.Button.Left)
            {
                Vector2f worldPos = _window.MapPixelToCoords(new Vector2i(e.X, e.Y));
                SelectTileAt(worldPos);
            }
            else if (e.Button == Mouse.Button.Right)
            {
                SelectTileAt(null);
            }

        };

        _window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button != Mouse.Button.Middle)
                return;

            _isMovingCam = true;
            _lastPos = new Vector2f(e.X, e.Y);
        };

        _window.MouseButtonReleased += (s, e) =>
        {
            if (e.Button != Mouse.Button.Middle)
                return;

            _isMovingCam = false;
        };

        _window.MouseMoved += (s, e) =>
        {
            if (!_isMovingCam)
                return;

            Vector2f currentPos = new Vector2f(e.X, e.Y);

            Vector2f offset = (_lastPos - currentPos) * _zoom;

            _view.Move(offset);

            _lastPos = currentPos;
        };

        _window.KeyPressed += (s, e) =>
        {
            if (e.Code == Keyboard.Key.Space)
                _world.Paused = !world.Paused;
        };

        RegisterDefaultRenders();
    }

    public void Dispose()
    {
        _window.Dispose();
        _grass.Dispose();
        _castle.Dispose();
        _dwarf.Dispose();
        _mountain.Dispose();
        _smallMountain.Dispose();

        _highlight.Dispose();
    }

    public void Render()
    {
        _window.Clear(new Color(135, 206, 235));

        _window.DispatchEvents();

        HighlightSelectedTile();

        _window.SetView(_view);

        var (startPos, endPos) = GetBounds();

        for (int y = startPos.Y; y <= endPos.Y; y++)
        {
            if (y < 0 || y > _world.Height - 1)
                continue;

            for (int x = startPos.X; x <= endPos.X; x++)
            {
                if (x < 0 || x > _world.Width - 1)
                    continue;

                ITileView tile = _world[x, y];

                Transform t = Transform.Identity;
                t.Translate(new Vector2f(tile.Position.X * 32, tile.Position.Y * 32));
                RenderStates rs = new(t);

                _window.Draw(_grass, rs);

                foreach (IEntity entity in tile.Contents)
                {
                    if (entity.RenderType != null)
                    {
                        _renderList.Add((entity, rs));
                    }
                }
            }
        }

        // TODO: Inplace sorting
        foreach (var (entity, rs) in _renderList
            .OrderBy(e => e.Ent.Layer)
            .ThenBy(e => e.Ent.Position.Y)
            .ThenBy(e => e.Ent.Position.X))
        {
            _renders[entity.RenderType!](entity, rs);
        }

        _debug.Tick();

        foreach (Highlight highlights in _debug.TileHighlights)
        {
            Transform t = Transform.Identity;
            t.Translate(new Vector2f(highlights.Position.X * 32, highlights.Position.Y * 32));
            RenderStates rs = new(t);

            _window.Draw(_highlight, rs);
        }

        _window.Display();
        _renderList.Clear();
    }

    private Vector GetTileAt(Vector2f position)
    {
        return new Vector((int)Math.Floor(position.X / 32), (int)Math.Floor(position.Y / 32));
    }

    private void SelectTileAt(Vector2f? position)
    {
        if (position == null)
        {
            _selectedTile = null;
            Console.Clear();
            return;
        }

        Vector tilePosition = GetTileAt(position.Value);

        if (tilePosition.X < 0 || tilePosition.X > _world.Width - 1 || tilePosition.Y < 0 || tilePosition.Y > _world.Height - 1)
        {
            _selectedTile = null;
            Console.Clear();
            return;
        }

        _selectedTile = _world[tilePosition];
    }

    private (Vector leftUpper, Vector rightBottom) GetBounds()
    {
        Vector2f a1 = _window.MapPixelToCoords(new Vector2i(0, 0));
        Vector2f a2 = _window.MapPixelToCoords(new Vector2i((int)_window.Size.X - 1, (int)_window.Size.Y - 1));

        Vector t1 = GetTileAt(a1);
        Vector t2 = GetTileAt(a2);

        return (t1, t2);
    }

    private void HighlightSelectedTile()
    {
        if (_selectedTile == null)
        {
            if (_lastSelectedTile != null)
            {
                _lastSelectedTile = null;
                _debug.Clear();
            }

            return;
        }

        if (_lastTick != _world.CurrentTick || _selectedTile != _lastSelectedTile)
        {
            _debug.Clear();
            _debug.HighlightTileAt(_selectedTile.Position, _world.CurrentTick);
            Console.Clear();
            _consoleInterface.DisplayTileInfo(_selectedTile);

            _lastTick = _world.CurrentTick;
            _lastSelectedTile = _selectedTile;
        }
    }

    private static Texture LoadTexture(string path)
    {
        Texture result = new(path);
        result.GenerateMipmap();
        result.Smooth = false;

        return result;
    }

    private void RegisterRender(string entityType, Action<IEntity, RenderStates> callback)
    {
        _renders[entityType] = callback;
    }

    private void RegisterDefaultRenders()
    {
        RegisterRender("building", DrawBuilding);
        RegisterRender("dwarf", DrawDwarf);
        RegisterRender("mountain", DrawMountain);
    }

    private void DrawBuilding(IEntity building, RenderStates states)
    {
        _window.Draw(_castle, states);
    }

    private void DrawDwarf(IEntity creature, RenderStates states)
    {
        _window.Draw(_dwarf, states);
    }

    private void DrawMountain(IEntity ground, RenderStates states)
    {
        SizeState? size = ground.GetState<SizeState>();
        if (size == null)
            return;

        if (size.Value.Size <= 8)
        {
            _window.Draw(_smallMountain, states);
        }
        else if (size.Value.Size >= 16)
        {
            _window.Draw(_mountain, states);
        }
    }
}
