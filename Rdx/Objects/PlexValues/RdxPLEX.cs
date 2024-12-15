using System.Collections;

namespace Rdx.Objects.PlexValues;

public abstract class RdxPLEX : RdxObject, IEnumerable<object>
{
    protected readonly List<object> Items;

    public abstract int Count { get; } 
    
    protected RdxPLEX(
        List<object> items,
        long replicaId, 
        long version,
        long currentReplicaId) 
        : base(replicaId, version, currentReplicaId)
    {
        Items = items;
    }

    public IEnumerator<object> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}