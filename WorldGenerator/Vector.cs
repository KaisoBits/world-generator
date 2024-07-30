namespace WorldGenerator;

public readonly struct Vector : IEquatable<Vector>
{
    public static Vector Zero { get; } = new Vector();
    public static Vector Up { get; } = new Vector(-1, 0);
    public static Vector Down { get; } = new Vector(1, 0);
    public static Vector Left { get; } = new Vector(-1, 0);
    public static Vector Right { get; } = new Vector(1, 0);

    public int X { get; }
    public int Y { get; }

    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector operator *(Vector a, int scalar) => new(a.X * scalar, a.Y * scalar);

    public static Vector operator *(int scalar, Vector a) => new(a.X * scalar, a.Y * scalar);

    public static Vector operator /(Vector a, int scalar) => new(a.X / scalar, a.Y / scalar);

    public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(Vector a, Vector b) => !(a == b);

    public Vector Abs() => new(Math.Abs(X), Math.Abs(Y));

    public float SimpleLen() => Math.Abs(X) + Math.Abs(Y);

    public override bool Equals(object? obj) => obj is Vector other && this == other;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public bool Equals(Vector other) => other.X == X && other.Y == Y;

    public override string ToString() => $"({X}, {Y})";
}
