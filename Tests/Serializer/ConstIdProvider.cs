using Rdx.Objects;

namespace Tests.Serializer;

public class ConstIdProvider : IReplicaIdProvider
{
    private readonly long id;

    public ConstIdProvider(long? id = null)
    {
        this.id = id ?? Random.Shared.NextInt64();
    }

    public long GetReplicaId()
    {
        return id;
    }
}