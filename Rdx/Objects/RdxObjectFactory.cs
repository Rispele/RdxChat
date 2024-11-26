namespace Rdx.Objects;

public class RdxObjectFactory
{
    private const long InitialVersion = 0;
    private readonly IReplicaIdProvider replicaIdProvider;

    public RdxObjectFactory(IReplicaIdProvider replicaIdProvider)
    {
        this.replicaIdProvider = replicaIdProvider;
    }

    public RdxValue<T> Value<T>(T value, long replicaId, long version)
    {
        return new RdxValue<T>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    public RdxValue<T> NewValue<T>(T value)
    {
        var replicaId = replicaIdProvider.GetReplicaId();
        return new RdxValue<T>(value, replicaId, InitialVersion, replicaId);
    }

    public RdxTuple<T1, T2> Tuple<T1, T2>(T1 first, T2 second, long replicaId, long version)
        where T1 : RdxObject
        where T2 : RdxObject
    {
        return new RdxTuple<T1, T2>(first, second, replicaId, version, replicaIdProvider.GetReplicaId());
    }
    
    public RdxTuple<T1, T2> NewTuple<T1, T2>(T1 first, T2 second)
        where T1 : RdxObject
        where T2 : RdxObject
    {
        var replicaId = replicaIdProvider.GetReplicaId();
        return new RdxTuple<T1, T2>(first, second, replicaId, InitialVersion, replicaId);
    }

    public RdxTriple<T1, T2, T3> Triple<T1, T2, T3>(T1 first, T2 second, T3 third, long replicaId, long version)
        where T1 : RdxObject
        where T2 : RdxObject
        where T3 : RdxObject
    {
        return new RdxTriple<T1, T2, T3>(first, second, third, replicaId, version, replicaIdProvider.GetReplicaId());
    }
    
    public RdxTriple<T1, T2, T3> NewTriple<T1, T2, T3>(T1 first, T2 second, T3 third)
        where T1 : RdxObject
        where T2 : RdxObject
        where T3 : RdxObject
    {
        var replicaId = replicaIdProvider.GetReplicaId();
        return new RdxTriple<T1, T2, T3>(first, second, third, replicaId, InitialVersion, replicaId);
    }
}