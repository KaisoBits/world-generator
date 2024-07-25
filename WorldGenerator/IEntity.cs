﻿namespace WorldGenerator;

public interface IEntity
{
    Layer Layer { get; }
    int X { get; }
    int Y { get; }

    ITileView CurrentTile { get; }

    void GatherConditions();
    void Think();

    void AcceptRenderer<T>(IRendererVisitor<T> renderer, T state);
}
