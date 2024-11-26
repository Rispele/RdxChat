using Rdx.Extensions;

namespace Rdx.Objects.PlexValues;

public class RdxTuple<T1, T2> : RdxObject
    where T1 : RdxObject
    where T2 : RdxObject
{
    private T1 first;

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

    private T2 second;

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

    public RdxTuple(T1 first, T2 second, long replicaId, long version, long currentReplicaId)
        : base(replicaId, version, currentReplicaId)
    {
        first.EnsureNotNull();
        second.EnsureNotNull();
        
        this.first = first;
        this.second = second;
    }

    public override string Serialize()
    {
        return $"<{SerializeStamp()} {first.Serialize()}:{second.Serialize()}>";
    }
}