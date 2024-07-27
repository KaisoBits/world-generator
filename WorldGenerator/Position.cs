namespace WorldGenerator;

public readonly struct Position : IEquatable<Position>
{
    public static Position Zero { get; } = new Position();
    public static Position Up { get; } = new Position(-1, 0);
    public static Position Down { get; } = new Position(1, 0);
    public static Position Left { get; } = new Position(-1, 0);
    public static Position Right { get; } = new Position(1, 0);

    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.X + b.X, a.Y + b.Y);
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y);
    }

    public static Position operator *(Position a, int scalar)
    {
        return new Position(a.X * scalar, a.Y * scalar);
    }

    public static Position operator *(int scalar, Position a)
    {
        return new Position(a.X * scalar, a.Y * scalar);
    }

    public static Position operator /(Position a, int scalar)
    {
        return new Position(a.X / scalar, a.Y / scalar);
    }

    public static bool operator ==(Position a, Position b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Position a, Position b)
    {
        return !(a == b);
    }

    public Position Abs()
    {
        return new Position(Math.Abs(X), Math.Abs(Y));
    }

    public float SimpleLen() => Math.Abs(X) + Math.Abs(Y);

    public override bool Equals(object? obj)
    {
        if (obj is Position other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public bool Equals(Position other)
    {
        return other.X == X && other.Y == Y;
    }


    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
