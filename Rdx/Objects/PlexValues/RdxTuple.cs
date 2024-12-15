using Rdx.Extensions;
using Rdx.Serialization.Attributes.XPleSerializers;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxTuple<T1, T2> : RdxPLEX
    where T1 : RdxObject
    where T2 : RdxObject
{
    public override int Count => 2;

    public T1 First
    {
        get => (Items[0] as T1)!;
        set
        {
            value.EnsureNotNull();
            Items[0] = value;

            UpdateObject();
        }
    }

    public T2 Second
    {
        get => (Items[1] as T2)!;
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