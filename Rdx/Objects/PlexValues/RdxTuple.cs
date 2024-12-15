using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxTuple<T1, T2> : RdxPLEX 
    where T1 : notnull
    where T2 : notnull
{
    public override int Count => 2;

    public T1 First
    {
        get => (T1)Items[0];
        set
        {
            value.EnsureNotNull();
            Items[0] = value;

            UpdateObject();
        }
    }

    public T2 Second
    {
        get => (T2)Items[1];
        set
        {
            value.EnsureNotNull();
            Items[1] = value;

            UpdateObject();
        }
    }

    public RdxTuple(T1 first, T2 second, long replicaId, long version, long currentReplicaId)
        : base([first, second], replicaId, version, currentReplicaId)
    {
        first.EnsureNotNull();
        second.EnsureNotNull();
    }
}