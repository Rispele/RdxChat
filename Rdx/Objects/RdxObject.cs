using JetBrains.Annotations;

namespace Rdx.Objects;

public abstract class RdxObject
{
    private long CurrentReplicaId { get; [UsedImplicitly] set; }
    private bool updated;
    
    public long ReplicaId { get; private set; }
    public long Version { get; private set; }

    protected RdxObject(long replicaId, long version, long currentReplicaId)
    {
        ReplicaId = replicaId;
        Version = version;

        CurrentReplicaId = currentReplicaId;
    }

    protected void UpdateObject()
    {
        if (updated)
        {
            return;
        }

        ReplicaId = CurrentReplicaId;
        Version++;
        updated = true;
    }
}