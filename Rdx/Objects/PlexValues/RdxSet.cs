using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxSetSerializer]
public class RdxSet<T>(
    HashSet<T> items,
    long replicaId,
    long version,
    long currentReplicaId)
    : RdxPLEX(replicaId, version, currentReplicaId)
{
    private readonly HashSet<T> items = items.ToHashSet();

    public override int Count => items.Count;

    public override IEnumerator<object> GetEnumerator()
    {
        return items.Cast<object>().GetEnumerator();
    }

    public bool Contains(T item)
    {
        return items.Contains(item);
    }

    public bool Add(T item)
    {
        var added = items.Add(item);
        if (added) UpdateObject();
        return added;
    }

    public bool Remove(T item)
    {
        var removed = items.Remove(item);
        if (removed) UpdateObject();
        return removed;
    }
}