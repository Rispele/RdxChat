using System.Collections;
using JetBrains.Annotations;
using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxXPle<T> : RdxPLEX
{
    public override int Count => Items.Count;

    public RdxXPle(
        List<object> items,
        long replicaId,
        long version,
        long currentReplicaId)
        : base(items, replicaId, version, currentReplicaId)
    {
    }

    public void Add(T rdxObject)
    {
        Items.Add(rdxObject!);
        UpdateObject();
    }
    
    [Obsolete("Not supported by rdx merge")]
    public object RemoveAt(int index)
    {
        var value = Items[index];
        
        Items.RemoveAt(index);
        UpdateObject();

        return value;
    }

    public T this[int index]
    {
        get => (T) Items[index]!;
        set
        {
            value.EnsureNotNull();
            Items[index] = value;
           
            UpdateObject();
        }
    }
}