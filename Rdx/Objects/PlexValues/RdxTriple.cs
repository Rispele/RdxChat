using Rdx.Extensions;
using Rdx.Serialization.Attributes.XPleSerializers;

namespace Rdx.Objects.PlexValues;

[RdxXPleSerializer]
public class RdxTriple<T1, T2, T3> : RdxPLEX
    where T1 : RdxObject
    where T2 : RdxObject
    where T3 : RdxObject
{
    public override int Count => 3;
    
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

    public T3 Third
    {
        get => (Items[2] as T3)!;
        set
        {
            value.EnsureNotNull();
            Items[2] = value;

            UpdateObject();
        }
    }

    public RdxTriple(T1 first, T2 second, T3 third, long replicaId, long version, long currentReplicaId)
        : base([first, second, third], replicaId, version, currentReplicaId)
    {
        first.EnsureNotNull();
        second.EnsureNotNull();
        third.EnsureNotNull();
    }
}