﻿using Microsoft.Extensions.Hosting;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WorldGenerator.States;

namespace WorldGenerator.SFML;

public sealed class SFMLRenderer : IRenderer, IDisposable
{
    private readonly RenderWindow _window;
    private readonly View _view;

    private readonly Sprite _grass = LoadSprite("Resources/grass2.png");
    private readonly Sprite _castle = LoadSprite("Resources/village.png");
    private readonly Sprite _dwarf = LoadSprite("Resources/dwarf.png");
    private readonly Sprite _mountain = LoadSprite("Resources/mountain.png");
    private readonly Sprite _smallMountain = LoadSprite("Resources/stone.png");
    private readonly Sprite _berries_full = LoadSprite("Resources/berries_full.png");
    private readonly Sprite _berries_Empty = LoadSprite("Resources/berries_empty.png");
    private readonly Sprite _field = LoadSprite("Resources/field.png");
    private readonly Sprite _wall0 = LoadSprite("Resources/wall-0.png");
    private readonly Sprite _wall1 = LoadSprite("Resources/wall-1.png");
    private readonly Sprite _wall21 = LoadSprite("Resources/wall-2-1.png");
    private readonly Sprite _wall22 = LoadSprite("Resources/wall-2-2.png");
    private readonly Sprite _wall3 = LoadSprite("Resources/wall-3.png");
    private readonly Sprite _wall4 = LoadSprite("Resources/wall-4.png");

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

    public SFMLRenderer(
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
                if (_currentZ != newZ)
                {
                    _currentZ = newZ;
                    Console.WriteLine("Z-Level is: {0}", _currentZ);
                }
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
                Vector2f worldPos = _window.MapPixelToCoords(new Vector2i(e.X, e.Y));
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

        for (int y = startPos.Y; y <= endPos.Y; y++)
        {
            if (y < 0 || y > _world.Height - 1)
                continue;

            for (int x = startPos.X; x <= endPos.X; x++)
            {
                if (x < 0 || x > _world.Width - 1)
                    continue;

                ITileView tile = _world[x, y, _currentZ];

                Transform t = Transform.Identity;
                t.Translate(new Vector2f(tile.Position.X * 64 + 32, tile.Position.Y * 64 + 32));
                RenderStates rs = new(t);

                DrawTile(tile, rs);
            }
        }

        _debug.Tick();

        foreach (Highlight highlights in _debug.TileHighlights.Where(th => th.Position.Z == _currentZ))
        {
            Transform t = Transform.Identity;
            t.Translate(new Vector2f(highlights.Position.X * 64, highlights.Position.Y * 64));
            RenderStates rs = new(t);

            _window.Draw(_highlight, rs);
        }

        _window.Display();
    }

    private Vector GetTileAt(Vector2f position)
    {
        return new Vector((int)Math.Floor(position.X / 64), (int)Math.Floor(position.Y / 64), _currentZ);
    }

    private void DrawTile(ITileView tile, RenderStates renderStates, int iteration = 0)
    {
        if (iteration > 6)
            return;

        if (tile.HasWall)
            DrawWall(tile, renderStates);
        else if (tile.HasFloor)
            _window.Draw(_grass, renderStates);

        if (!tile.HasFloor && !tile.HasWall && tile.Position.Z > 0)
        {
            DrawTile(_world[tile.Position - new Vector(0, 0, 1)], renderStates, iteration + 1);
            _window.Draw(_fog, renderStates);
        }

        foreach (IEntity entity in tile.Contents)
            _renders[entity.EntityType.FullIdentifier](entity, renderStates);
    }

    private void SelectTileAt(Vector2f? position)
    {
        if (position == null)
        {
            _selectionService.UnselectTile();
            Console.Clear();
            return;
        }

        Vector tilePosition = GetTileAt(position.Value);

        if (tilePosition.X < 0 || tilePosition.X > _world.Width - 1 || tilePosition.Y < 0 || tilePosition.Y > _world.Height - 1)
        {
            _selectionService.UnselectTile();
            Console.Clear();
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

    private void DrawWall(ITileView tile, RenderStates rs)
    {
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
        Texture tex = LoadTexture(path);

        Sprite result = new(tex);
        result.Origin = (Vector2f)tex.Size / 2;

        return result;
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
