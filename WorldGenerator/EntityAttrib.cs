namespace WorldGenerator;

public readonly record struct EntityAttrib(
    EntityAttribType Type, IReadOnlyDictionary<string, string> Parameters);
