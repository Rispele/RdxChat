namespace Rdx.Objects;

public class RdxObjectFactory
{
    private readonly IReplicaIdProvider replicaIdProvider;

    public RdxObjectFactory(IReplicaIdProvider replicaIdProvider)
    {
        this.replicaIdProvider = replicaIdProvider;
    }

    public RdxValue<T> CreateValue<T>(T value, long replicaId, long version)
    {
        return new RdxValue<T>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }
    
    
    public RdxTuple<T1, T2> CreateTuple<T1, T2>(T1 first, T2 second, long replicaId, long version)
        where T1: RdxObject
        where T2: RdxObject
    {
        return new RdxTuple<T1, T2>(first, second, replicaId, version, replicaIdProvider.GetReplicaId());
    }
    
    public RdxTriple<T1, T2, T3> CreateTriple<T1, T2, T3>(T1 first, T2 second, T3 third, long replicaId, long version)
        where T1: RdxObject
        where T2: RdxObject
        where T3: RdxObject
    {
        return new RdxTriple<T1, T2, T3>(first, second, third, replicaId, version, replicaIdProvider.GetReplicaId());
    }
}