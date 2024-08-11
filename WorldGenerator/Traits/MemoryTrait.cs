using WorldGenerator.Memories;

namespace WorldGenerator.Traits;

public class MemoryTrait : Trait<NullTraitData>
{
    public IReadOnlyCollection<IEntityMemory> Memories => _memories;
    private readonly List<IEntityMemory> _memories = [];

    public void Remember(IEntityMemory memory)
    {
        _memories.Add(memory);
    }
}
