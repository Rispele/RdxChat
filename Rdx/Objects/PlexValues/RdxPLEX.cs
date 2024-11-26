namespace Rdx.Objects.PlexValues;

public abstract class RdxPLEX : RdxObject
{
    protected readonly List<RdxObject> Items;
    
    protected RdxPLEX(
        List<RdxObject> items,
        long replicaId, 
        long version,
        long currentReplicaId) 
        : base(replicaId, version, currentReplicaId)
    {
        Items = items;
    }
}