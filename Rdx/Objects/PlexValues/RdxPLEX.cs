using System.Collections;

namespace Rdx.Objects.PlexValues;

public abstract class RdxPLEX : RdxObject, IEnumerable<RdxObject>
{
    protected readonly List<RdxObject> Items;

    public abstract int Count { get; } 
    
    protected RdxPLEX(
        List<RdxObject> items,
        long replicaId, 
        long version,
        long currentReplicaId) 
        : base(replicaId, version, currentReplicaId)
    {
        Items = items;
    }

    public IEnumerator<RdxObject> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}