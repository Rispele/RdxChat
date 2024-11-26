namespace Rdx.Objects;

public abstract class RdxObject
{
    public Guid ReplicaId { get; private set; }
    public long Version { get; private set; }

    protected RdxObject(Guid replicaId, long version)
    {
        ReplicaId = replicaId;
        Version = version;
    }

    protected void UpdateObject(Guid? replicaId = null)
    {
        if (replicaId is not null)
        {
            ReplicaId = replicaId.Value;
        }

        Version++;
    }

    public override string ToString()
    {
        return $"@{ReplicaId}-{Version}";
    }
}