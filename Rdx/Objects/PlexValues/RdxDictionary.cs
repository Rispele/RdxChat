using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxDictionarySerializer]
public class RdxDictionary<TKey, TValue>(
    IDictionary<TKey, TValue> dictionary,
    long replicaId,
    long version,
    long currentReplicaId)
    : RdxPLEX(replicaId, version, currentReplicaId)
    where TKey : IComparable<TKey>
{
    private readonly Dictionary<TKey, TValue> dictionary = dictionary.ToDictionary();
    public override int Count => dictionary.Count;

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set
        {
            value.EnsureNotNull();
            if (dictionary.TryAdd(key, value)) UpdateObject();
        }
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public override IEnumerator<object> GetEnumerator()
    {
        return dictionary
            .Select(t => ((object)t.Key, (object)t.Value!))
            .Cast<object>()
            .GetEnumerator();
    }
}