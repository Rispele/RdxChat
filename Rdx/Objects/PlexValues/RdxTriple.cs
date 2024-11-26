using Rdx.Extensions;

namespace Rdx.Objects.PlexValues;

public class RdxTriple<T1, T2, T3> : RdxObject
    where T1 : RdxObject
    where T2 : RdxObject
    where T3 : RdxObject
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
    
    private T3 third;

    public T3 Third
    {
        get => third;
        set
        {
            value.EnsureNotNull();
            third = value;
            
            UpdateObject();
        }
    }

    public RdxTriple(T1 first, T2 second, T3 third, long replicaId, long version, long currentReplicaId)
        : base(replicaId, version, currentReplicaId)
    {
        first.EnsureNotNull();
        second.EnsureNotNull();
        third.EnsureNotNull();
        
        this.first = first;
        this.second = second;
        this.third = third;
    }

    public override string Serialize()
    {
        return $"<{SerializeStamp()} {first.Serialize()}:{second.Serialize()}:{third.Serialize()}>";
    }
}