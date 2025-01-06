using System.Collections;

namespace Rdx.Objects.PlexValues;

public abstract class RdxPLEX(long replicaId, long version, long currentReplicaId)
    : RdxObject(replicaId, version, currentReplicaId), IEnumerable<object>
{
    public abstract int Count { get; }

    public abstract IEnumerator<object> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}