namespace Rdx.Objects.PlexValues;

public class RdxXple : RdxPLEX
{
    public RdxXple(
        List<RdxObject> items,
        long replicaId,
        long version,
        long currentReplicaId)
        : base(items, replicaId, version, currentReplicaId)
    {
    }

    public void Add(RdxObject rdxObject)
    {
        Items.Add(rdxObject);
    }

    public RdxObject RemoveAt(int index)
    {
        var value = Items[index];
        
        Items.RemoveAt(index);

        return value;
    }

    public RdxObject this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    } 
    
    public override string Serialize()
    {
        return $"<{SerializeStamp()} {string.Join(":", Items.Select(item => item.Serialize()))}>";
    }
}