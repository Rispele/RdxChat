using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxXPle<T> : RdxPLEX
{
    private readonly List<T> items;
    public override int Count => items.Count;

    public RdxXPle(
        List<T> items,
        long replicaId,
        long version,
        long currentReplicaId)
        : base(replicaId, version, currentReplicaId)
    {
        this.items = items;
    }

    public void Add(T rdxObject)
    {
        items.Add(rdxObject!);
        UpdateObject();
    }
    
    [Obsolete("Not supported by rdx merge")]
    public object RemoveAt(int index)
    {
        var value = items[index];
        
        items.RemoveAt(index);
        UpdateObject();

        return value;
    }

    public T this[int index]
    {
        get => (T) items[index]!;
        set
        {
            value.EnsureNotNull();
            items[index] = value;
           
            UpdateObject();
        }
    }

    public override int GetHashCode()
    {
        return items[0].GetHashCode();
    }

    public override IEnumerator<object> GetEnumerator()
    {
        return items.Cast<object>().GetEnumerator();
    }
}