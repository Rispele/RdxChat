using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxTupleSerializer]
public class RdxTuple<T1, T2> : RdxPLEX
    where T1 : notnull
    where T2 : notnull
{
    private T1 first;

    private T2 second;

    public RdxTuple(T1 first, T2 second, long replicaId, long version, long currentReplicaId)
        : base(replicaId, version, currentReplicaId)
    {
        first.EnsureNotNull();
        second.EnsureNotNull();

        this.first = first;
        this.second = second;
    }

    public override int Count => 2;

    public T1 First
    {
        get => first;
        set
        {
            value.EnsureNotNull();
            first = value;

            UpdateObject();
        }
    }

    public T2 Second
    {
        get => second;
        set
        {
            value.EnsureNotNull();
            second = value;

            UpdateObject();
        }
    }

    public override IEnumerator<object> GetEnumerator()
    {
        yield return First;
        yield return Second;
    }

    public override int GetHashCode()
    {
        return First.GetHashCode();
    }
}