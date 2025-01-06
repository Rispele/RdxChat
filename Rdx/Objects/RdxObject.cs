using JetBrains.Annotations;

namespace Rdx.Objects;

public abstract class RdxObject
{
    private bool updated;

    protected RdxObject(long replicaId, long version, long currentReplicaId)
    {
        ReplicaId = replicaId;
        Version = version;

        CurrentReplicaId = currentReplicaId;
    }

    private long CurrentReplicaId { get; [UsedImplicitly] set; }

    public long ReplicaId { get; private set; }
    public long Version { get; private set; }

    protected void UpdateObject()
    {
        if (updated) return;

        ReplicaId = CurrentReplicaId;
        Version++;
        updated = true;
    }
}