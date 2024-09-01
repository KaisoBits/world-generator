using Microsoft.Extensions.Hosting;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator.States;

namespace WorldGenerator.SFML;

public sealed class IsometricRenderer : IRenderer, IDisposable
{
    private readonly RenderWindow _window;
    private readonly View _view;

    private static readonly Color _topWallColor = new Color(220, 220, 220);

    private readonly Sprite _grass = LoadSprite("Resources/grass2.png", _topWallColor);
    private readonly Sprite _castle = LoadSprite("Resources/village.png");
    private readonly Sprite _dwarf = LoadSprite("Resources/dwarf.png");
    private readonly Sprite _mountain = LoadSprite("Resources/mountain.png");
    private readonly Sprite _smallMountain = LoadSprite("Resources/stone.png");
    private readonly Sprite _field = LoadSprite("Resources/field.png");
    private readonly Sprite _wall0 = LoadSprite("Resources/wall-0.png", _topWallColor);
    private readonly Sprite _wall1 = LoadSprite("Resources/wall-1.png", _topWallColor);
    private readonly Sprite _wall21 = LoadSprite("Resources/wall-2-1.png", _topWallColor);
    private readonly Sprite _wall22 = LoadSprite("Resources/wall-2-2.png", _topWallColor);
    private readonly Sprite _wall3 = LoadSprite("Resources/wall-3.png", _topWallColor);
    private readonly Sprite _wall4 = LoadSprite("Resources/wall-4.png", _topWallColor);
    private readonly Texture _wallTex = LoadTexture("Resources/stone_wall.png", true);
    private readonly Texture _grassTex = LoadTexture("Resources/grass2.png", true);

    private const float _floorDepth = 10;

    private readonly float _perspectiveMultiplier = (float)Math.Sqrt(2);

    private readonly RectangleShape _fog = new(new Vector2f(64, 64))
    {
        FillColor = new Color(135, 206, 235, 90),
        Origin = new Vector2f(32, 32)
    };

    private readonly Shape _highlight = new RectangleShape(new Vector2f(64, 64))
    {
        FillColor = Color.Transparent,
        OutlineColor = Color.Red,
        OutlineThickness = -3,
    };

    private readonly Dictionary<string, Action<IEntity, RenderStates>> _renders = [];

    private readonly World _world;
    private readonly WorldFacade _worldFacade;
    private readonly DebugOverlay _debug;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ConsoleInterface _consoleInterface;
    private readonly SelectionService _selectionService;

    private bool _isMovingCam = false;
    private Vector2f _lastPos = new();
    private float _zoom = 1.0f;

    private int _currentZ = 0;

    private bool _ctrlPressed = false;

    private ITileView? _lastSelectedTile = null;

    public IsometricRenderer(
        World world,
        WorldFacade worldFacade,
        DebugOverlay overlay,
        IHostApplicationLifetime lifetime,
        EventBus eventBus,
        ConsoleInterface consoleInterface,
        SelectionService selectionService)
    {
        _world = world;
        _worldFacade = worldFacade;
        _debug = overlay;
        _lifetime = lifetime;
        _consoleInterface = consoleInterface;
        _selectionService = selectionService;

        ContextSettings settings = new()
        {
            AntialiasingLevel = 4
        };
        _window = new(new VideoMode(Math.Min((uint)world.Width * 64, 1280), Math.Min((uint)world.Height * 64, 720)), "World generator", Styles.Default, settings);
        _view = new View(new Vector2f(_window.Size.X / 2, _window.Size.Y / 2), new Vector2f(_window.Size.X, _window.Size.Y));
        _window.SetView(_view);

        _window.Closed += (s, e) => _lifetime.StopApplication();
        _window.Resized += (s, e) => _view.Size = new Vector2f(e.Width, e.Height) * _zoom;
        _window.MouseWheelScrolled += (s, e) =>
        {
            if (_ctrlPressed)
            {
                int newZ = Math.Clamp(_currentZ + (int)e.Delta, 0, _world.Depth - 1);
                _currentZ = newZ;
                return;
            }

            float multiplier = Math.Abs(e.Delta);
            float ratio = e.Delta < 0 ? 1.25f * multiplier : 0.8f / multiplier;
            _zoom = Math.Clamp(_zoom * ratio, 0.2f, 3.0f);

            _view.Size = (Vector2f)_window.Size * _zoom;
        };

        _window.MouseButtonPressed += (s, e) =>
        {
            if (e.Button == Mouse.Button.Left)
            {
                Vector2f worldPos = Map2DCoordsToIsometric(_window.MapPixelToCoords(new Vector2i(e.X, e.Y)));
                SelectTileAt(worldPos);
                _consoleInterface.RunCommand("mine");
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

            if (e.Code == Keyboard.Key.LControl)
                _ctrlPressed = true;

        };

        _window.KeyReleased += (s, e) =>
        {
            if (e.Code == Keyboard.Key.LControl)
                _ctrlPressed = false;

        };

        RegisterDefaultRenders();

        _consoleInterface.StartTakingInput();
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

        _consoleInterface.ProcessCommands();

        HighlightSelectedTile();

        _window.SetView(_view);

        var (startPos, endPos) = GetBounds();
        // TODO: Fix bounds
        startPos = new Vector(-999, -999, _currentZ);
        endPos = new Vector(999, 999, _currentZ);

        int size = Math.Max(_world.Size.X * 2, _world.Size.Y * 2);

        const int maxZOffset = 4;
        int renderedZLevel = Math.Max(_currentZ - maxZOffset, 0);

        while (renderedZLevel <= _currentZ)
        {
            int zOffset = (_currentZ - renderedZLevel);
            float pixelZOffset = (zOffset * 64.0f * _perspectiveMultiplier * 0.5f) + _floorDepth * zOffset;

            for (int i = 0; i < size; i++)
            {
                for (int drawingIndex = 0; drawingIndex <= i; drawingIndex++)
                {
                    Vector currentPos = new(drawingIndex, i - drawingIndex, renderedZLevel);
                    if (!_world.IsWithinBounds(currentPos))
                        continue;

                    ITileView tile = _world[currentPos];

                    Transform t = Transform.Identity;
                    t.Translate(new Vector2f(0, pixelZOffset));
                    t.Scale(new Vector2f(1, 0.5f));
                    t.Rotate(45);
                    t.Translate(new Vector2f(tile.Position.X * 64 + 32, tile.Position.Y * 64 + 32));
                    RenderStates rs = new(t);

                    DrawTile(tile, rs);
                }
            }

            if (renderedZLevel != _currentZ)
            {
                Transform fogT = Transform.Identity;
                fogT.Translate(_view.Center);
                fogT.Scale(_view.Size / 2);
                _window.Draw(_fog, new RenderStates(fogT));
            }

            renderedZLevel++;
        }

        _debug.Tick();

        foreach (Highlight highlights in _debug.TileHighlights.Where(th => th.Position.Z == _currentZ))
        {
            Transform t = Transform.Identity;
            t.Scale(new Vector2f(1, 0.5f));
            t.Rotate(45);
            t.Translate(new Vector2f(highlights.Position.X * 64, highlights.Position.Y * 64));

            RenderStates rs = new(t);

            _window.Draw(_highlight, rs);
        }

        _window.Display();
    }

    private Vector2f Map2DCoordsToIsometric(Vector2f coords)
    {
        Transform t = Transform.Identity;
        t.Rotate(-45);
        t.Scale(new Vector2f(1, 2f));

        return t.TransformPoint(coords);
    }

    private Vector GetTileAt(Vector2f position)
    {
        return new Vector((int)Math.Floor(position.X / 64), (int)Math.Floor(position.Y / 64), _currentZ);
    }

    private void DrawTile(ITileView tile, RenderStates renderStates)
    {
        if (tile.HasFloor)
            DrawFloor(tile, renderStates);
        if (tile.HasWall)
            DrawWall(tile, renderStates);

        renderStates.Transform.Translate(new Vector2f(-32, -32));
        renderStates.Transform.Rotate(-45);
        renderStates.Transform.Scale(new Vector2f(1, 2));

        foreach (IEntity entity in tile.Contents)
            _renders[entity.EntityType.FullIdentifier](entity, renderStates);
    }

    private void SelectTileAt(Vector2f? position)
    {
        if (position == null)
        {
            _selectionService.UnselectTile();
            return;
        }

        Vector tilePosition = GetTileAt(position.Value);

        if (tilePosition.X < 0 || tilePosition.X > _world.Width - 1 || tilePosition.Y < 0 || tilePosition.Y > _world.Height - 1)
        {
            _selectionService.UnselectTile();
            return;
        }

        _selectionService.SelectTile(tilePosition);
    }

    private (Vector leftUpper, Vector rightBottom) GetBounds()
    {
        Vector2f a1 = _window.MapPixelToCoords(new Vector2i(0, 0));
        Vector2f a2 = _window.MapPixelToCoords(new Vector2i((int)_window.Size.X - 1, (int)_window.Size.Y - 1));

        Vector t1 = GetTileAt(a1);
        Vector t2 = GetTileAt(a2);

        return (t1, t2);
    }

    private readonly VertexArray _sideWall = new(PrimitiveType.Quads, 4);

    private void DrawFloor(ITileView tile, RenderStates rs)
    {
        float height = -_floorDepth;

        Vector2f middlePoint = rs.Transform.TransformPoint(new Vector2f(32, 32));
        Vector2f leftPoint = rs.Transform.TransformPoint(new Vector2f(-32, 32));
        Vector2f rightPoint = rs.Transform.TransformPoint(new Vector2f(32, -32));

        _window.Draw(_grass, rs);

        if (!_world.TryGetTile(tile.Position + new Vector(0, 1, 0), out ITileView? leftTile) || (!leftTile.HasWall && !leftTile.HasFloor))
        {
            _sideWall[0] = new Vertex(middlePoint, Color.White, new Vector2f(64, 0));
            _sideWall[1] = new Vertex(leftPoint, Color.White, new Vector2f(0, 0));
            _sideWall[2] = new Vertex(leftPoint - new Vector2f(0, height), Color.White, new Vector2f(0, 64.0f * _perspectiveMultiplier * (height / 64.0f)));
            _sideWall[3] = new Vertex(middlePoint - new Vector2f(0, height), Color.White, new Vector2f(64, 64.0f * _perspectiveMultiplier * (height / 64.0f)));

            _window.Draw(_sideWall, new RenderStates(_grassTex));
        }

        if (!_world.TryGetTile(tile.Position + new Vector(1, 0, 0), out ITileView? rightTile) || (!rightTile.HasWall && !rightTile.HasFloor))
        {
            Color c = new Color(180, 180, 180);

            _sideWall[0] = new Vertex(middlePoint, c, new Vector2f(0, 0));
            _sideWall[1] = new Vertex(rightPoint, c, new Vector2f(64, 0));
            _sideWall[2] = new Vertex(rightPoint - new Vector2f(0, height), c, new Vector2f(64, 64.0f * _perspectiveMultiplier * (height / 64.0f)));
            _sideWall[3] = new Vertex(middlePoint - new Vector2f(0, height), c, new Vector2f(0, 64.0f * _perspectiveMultiplier * (height / 64.0f)));

            _window.Draw(_sideWall, new RenderStates(_grassTex));
        }
    }

    private void DrawWall(ITileView tile, RenderStates rs)
    {
        float height = -64 * _perspectiveMultiplier * 0.5f;

        var t = Transform.Identity;
        t.Translate(new Vector2f(0, height));
        t.Combine(rs.Transform);
        rs.Transform = t;

        Vector2f middlePoint = t.TransformPoint(new Vector2f(32, 32));
        Vector2f leftPoint = t.TransformPoint(new Vector2f(-32, 32));
        Vector2f rightPoint = t.TransformPoint(new Vector2f(32, -32));

        int neighborCount = _worldFacade.NeighborWallsCount(tile.Position);

        switch (neighborCount)
        {
            case 0:
                _window.Draw(_wall4, rs);
                break;
            case 1:
                if (_worldFacade.HasWall(tile.Position + Vector.Up))
                    rs.Transform.Rotate(-90);
                else if (_worldFacade.HasWall(tile.Position + Vector.Down))
                    rs.Transform.Rotate(90);
                else if (_worldFacade.HasWall(tile.Position + Vector.Left))
                    rs.Transform.Rotate(180);

                _window.Draw(_wall3, rs);
                break;

            case 2:
                if (_worldFacade.HasWall(tile.Position + Vector.Left) && _worldFacade.HasWall(tile.Position + Vector.Right))
                {
                    _window.Draw(_wall21, rs);
                }
                else if (_worldFacade.HasWall(tile.Position + Vector.Up) && _worldFacade.HasWall(tile.Position + Vector.Down))
                {
                    rs.Transform.Rotate(90);
                    _window.Draw(_wall21, rs);
                }
                else if (_worldFacade.HasWall(tile.Position + Vector.Up))
                {
                    if (_worldFacade.HasWall(tile.Position + Vector.Left))
                        rs.Transform.Rotate(-90);

                    _window.Draw(_wall22, rs);
                }
                else if (_worldFacade.HasWall(tile.Position + Vector.Down))
                {
                    rs.Transform.Rotate(180);
                    if (_worldFacade.HasWall(tile.Position + Vector.Right))
                        rs.Transform.Scale(-1, 1);

                    _window.Draw(_wall22, rs);
                }
                break;
            case 3:
                if (!_worldFacade.HasWall(tile.Position + Vector.Up))
                    rs.Transform.Rotate(180);
                else if (!_worldFacade.HasWall(tile.Position + Vector.Right))
                    rs.Transform.Rotate(-90);
                else if (!_worldFacade.HasWall(tile.Position + Vector.Left))
                    rs.Transform.Rotate(90);

                _window.Draw(_wall1, rs);
                break;
            case 4:
                _window.Draw(_wall0, rs);
                break;
            default:
                break;
        }

        if (!_world.TryGetTile(tile.Position + new Vector(0, 1, 0), out ITileView? leftTile) || !leftTile.HasWall)
        {
            _sideWall[0] = new Vertex(middlePoint, Color.White, new Vector2f(64, 0));
            _sideWall[1] = new Vertex(leftPoint, Color.White, new Vector2f(0, 0));
            _sideWall[2] = new Vertex(leftPoint - new Vector2f(0, height), Color.White, new Vector2f(0, 64.0f * _perspectiveMultiplier * (height / 64.0f)));
            _sideWall[3] = new Vertex(middlePoint - new Vector2f(0, height), Color.White, new Vector2f(64, 64.0f * _perspectiveMultiplier * (height / 64.0f)));

            _window.Draw(_sideWall, new RenderStates(_wallTex));
        }

        if (!_world.TryGetTile(tile.Position + new Vector(1, 0, 0), out ITileView? rightTile) || !rightTile.HasWall)
        {
            Color c = new Color(180, 180, 180);

            _sideWall[0] = new Vertex(middlePoint, c, new Vector2f(0, 0));
            _sideWall[1] = new Vertex(rightPoint, c, new Vector2f(64, 0));
            _sideWall[2] = new Vertex(rightPoint - new Vector2f(0, height), c, new Vector2f(64, 64.0f * _perspectiveMultiplier * (height / 64.0f)));
            _sideWall[3] = new Vertex(middlePoint - new Vector2f(0, height), c, new Vector2f(0, 64.0f * _perspectiveMultiplier * (height / 64.0f)));

            _window.Draw(_sideWall, new RenderStates(_wallTex));
        }
    }

    private void HighlightSelectedTile()
    {
        if (_selectionService.SelectedTile == null)
        {
            if (_lastSelectedTile != null)
            {
                _lastSelectedTile = null;
                _debug.Clear();
            }

            return;
        }

        if (_selectionService.SelectedTile != _lastSelectedTile)
        {
            _debug.Clear();
            _debug.HighlightTileAt(_selectionService.SelectedTile.Position, _world.CurrentTick);
        }
    }

    private static Sprite LoadSprite(string path)
    {
        return LoadSprite(path, Color.White);
    }

    private static Sprite LoadSprite(string path, Color color)
    {
        Texture tex = LoadTexture(path);

        Sprite result = new(tex);
        result.Origin = (Vector2f)tex.Size / 2;
        result.Color = color;

        return result;
    }

    private static Texture LoadTexture(string path, bool repeated = false)
    {
        Texture result = new(path);
        result.GenerateMipmap();
        result.Smooth = false;
        result.Repeated = repeated;

        return result;
    }

    private void RegisterRender(string entityType, Action<IEntity, RenderStates> callback)
    {
        _renders[entityType] = callback;
    }

    private void RegisterDefaultRenders()
    {
        RegisterRender("stock.building.fortress", DrawBuilding);
        RegisterRender("stock.creature.dwarf", DrawDwarf);
        RegisterRender("stock.terrain.mountain", DrawMountain);
        RegisterRender("stock.terrain.field", DrawField);
    }

    private void DrawBuilding(IEntity building, RenderStates states)
    {
        _window.Draw(_castle, states);
    }

    private void DrawDwarf(IEntity creature, RenderStates states)
    {
        _window.Draw(_dwarf, states);
    }

    private void DrawField(IEntity building, RenderStates states)
    {
        _window.Draw(_field, states);
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
