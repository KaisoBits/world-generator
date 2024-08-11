using System.Text.RegularExpressions;

namespace WorldGenerator;

public readonly partial struct EntityType
{
    public string Brand { get; private init; }
    public string Group { get; private init; }
    public string Entity { get; private init; }

    public string FullIdentifier { get; private init; }

    public bool Matches(string typeIdentifier) => Matches(Parse(typeIdentifier));

    public bool Matches(EntityType other)
    {
        return FullIdentifier == other.FullIdentifier || (
                (other.Brand == "*" || other.Brand == Brand) &&
                (other.Group == "*" || other.Group == Group) &&
                (other.Entity == "*" || other.Entity == Brand));
    }

    public override string ToString()
    {
        return FullIdentifier;
    }

    public static EntityType Parse(string entityType)
    {
        Match match = EntityTypeRegex().Match(entityType);
        if (!match.Success)
            throw new Exception($"Invalid entity type identifier: '{entityType}'");

        return new EntityType
        {
            Brand = match.Groups[1].Value,
            Group = match.Groups[2].Value,
            Entity = match.Groups[3].Value,
            FullIdentifier = entityType
        };
    }

    [GeneratedRegex(@"([a-z*]+?)\.([a-z*]+?)\.([a-z*]+)")]
    private static partial Regex EntityTypeRegex();
}
