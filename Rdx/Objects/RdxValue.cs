namespace Rdx.Objects;

/// <inheritdoc />
/// Possible values: string, int, double
public sealed class RdxValue<TValue> : RdxObject
{
    public TValue Value { get; private set; }

    public RdxValue(TValue value, Guid replicaId, long version = 0) 
        : base(replicaId, version)
    {
        EnsureValueIsNotNull(value);
        EnsureValueTypeIsAllowed(value);

        Value = value;
    }

    public void Update(TValue value, Guid? replicaId = null)
    {
        EnsureValueIsNotNull(value);
        
        Value = value;
        UpdateObject(replicaId);
    }

    private void EnsureValueTypeIsAllowed(TValue value)
    {
        if (!RdxValueConstants.RdxValueAllowedTypes.Contains(value?.GetType()))
        {
            throw new InvalidOperationException($"Could not create RdxValue with type: {value?.GetType()}");
        }
    }

    private void EnsureValueIsNotNull(TValue value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

    public override string ToString()
    {
        return $"{Value}{base.ToString()}";
    }
}