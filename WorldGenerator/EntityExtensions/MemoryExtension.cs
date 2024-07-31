﻿using WorldGenerator.Memories;

namespace WorldGenerator.EntityExtensions;

public class MemoryExtension : EntityExtension
{
    public IReadOnlyCollection<EntityMemory> Memories => _memories;
    private readonly List<EntityMemory> _memories = [];

    public void Remember(EntityMemory memory)
    {
        _memories.Add(memory);
    }
}
