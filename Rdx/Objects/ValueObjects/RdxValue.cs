using Rdx.Extensions;
using Rdx.Serialization.Attributes;

namespace Rdx.Objects.ValueObjects;

/// <inheritdoc />
/// Possible values: string, int, long, double, bool
[RdxValueSerializer]
public sealed class RdxValue<TValue> : RdxObject
    where TValue: notnull
{
    private TValue value;

    public TValue Value
    {
        get => value;
        set
        {
            value.EnsureNotNull();
            value.GetType().EnsureTypeAllowedAsRdxValueType();

            this.value = value;
            UpdateObject();
        }
    }

    public RdxValue(TValue value, long replicaId, long version, long currentReplicaId)
        : base(replicaId, version, currentReplicaId)
    {
        value.EnsureNotNull();
        value.GetType().EnsureTypeAllowedAsRdxValueType();

        this.value = value;
    }
}