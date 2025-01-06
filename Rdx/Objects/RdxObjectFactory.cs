using Rdx.Objects.PlexValues;
using Rdx.Objects.ValueObjects;

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
        where T : notnull
    {
        var replicaId = replicaIdProvider.GetReplicaId();
        return new RdxValue<T>(value, replicaId, InitialVersion, replicaId);
    }

    public RdxTuple<T1, T2> Tuple<T1, T2>(T1 first, T2 second, long replicaId, long version)
        where T1 : notnull
        where T2 : notnull
    {
        return new RdxTuple<T1, T2>(first, second, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    public RdxTuple<T1, T2> NewTuple<T1, T2>(T1 first, T2 second)
        where T1 : notnull
        where T2 : notnull
    {
        var replicaId = replicaIdProvider.GetReplicaId();
        return new RdxTuple<T1, T2>(first, second, replicaId, InitialVersion, replicaId);
    }
}