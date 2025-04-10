﻿namespace WorldGenerator;

public readonly struct Vector : IEquatable<Vector>
{
    public static Vector Zero { get; } = new Vector();
    public static Vector Up { get; } = new Vector(0, -1, 0);
    public static Vector Down { get; } = new Vector(0, 1, 0);
    public static Vector Left { get; } = new Vector(-1, 0, 0);
    public static Vector Right { get; } = new Vector(1, 0, 0);

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public Vector(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector operator *(Vector a, int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public static Vector operator *(int scalar, Vector a) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public static Vector operator /(Vector a, int scalar) => new(a.X / scalar, a.Y / scalar, a.Z / scalar);

    public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

    public static bool operator !=(Vector a, Vector b) => !(a == b);

    public Vector Abs() => new(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));

    public float SimpleLen() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

    public override bool Equals(object? obj) => obj is Vector other && this == other;

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public bool Equals(Vector other) => other.X == X && other.Y == Y && other.Z == Z;

    public override string ToString() => $"({X}, {Y}, {Z})";
}
