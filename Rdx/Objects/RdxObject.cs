namespace Rdx.Objects;

public abstract class RdxObject
{
    private readonly long currentReplicaId;
    private bool updated;
    
    public long ReplicaId { get; private set; }
    public long Version { get; private set; }

    protected RdxObject(long replicaId, long version, long currentReplicaId)
    {
        ReplicaId = replicaId;
        Version = version;

        this.currentReplicaId = currentReplicaId;
    }

    protected void UpdateObject()
    {
        if (updated)
        {
            return;
        }
        
        ReplicaId = currentReplicaId;
        Version++;
        updated = true;
    }

    public abstract string Serialize();

    protected string SerializeStamp()
    {
        return $"@{ReplicaId:X}-{Version}";
    }
}