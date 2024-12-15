using System.Collections;
using Rdx.Extensions;
using Rdx.Serialization.Attributes.XPleSerializers;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxXPle : RdxPLEX
{
    public override int Count => Items.Count;
    
    public RdxXPle(
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
        UpdateObject();
    }
    
    [Obsolete("Not supported by rdx merge")]
    public RdxObject RemoveAt(int index)
    {
        var value = Items[index];
        
        Items.RemoveAt(index);
        UpdateObject();

        return value;
    }

    public RdxObject this[int index]
    {
        get => Items[index];
        set
        {
            value.EnsureNotNull();
            Items[index] = value;
           
            UpdateObject();
        }
    }
}