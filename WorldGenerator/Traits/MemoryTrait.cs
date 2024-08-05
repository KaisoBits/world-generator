using WorldGenerator.Memories;

namespace WorldGenerator.Traits;

public class MemoryTrait : Trait<NullTraitData>
{
    public IReadOnlyCollection<EntityMemory> Memories => _memories;
    private readonly List<EntityMemory> _memories = [];

    public void Remember(EntityMemory memory)
    {
        _memories.Add(memory);
    }
}
