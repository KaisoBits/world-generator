using WorldGenerator.Traits;

namespace WorldGenerator;

public interface IIntentResolver
{
    void Resolve(IIntentResolverContext ctx);
}
