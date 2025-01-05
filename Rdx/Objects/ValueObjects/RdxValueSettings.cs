namespace Rdx.Objects.ValueObjects;

public static class RdxValueSettings
{
    public static readonly Type[] RdxValueAllowedTypes = [
        typeof(string),
        typeof(int),
        typeof(long),
        typeof(double),
        typeof(bool),
        typeof(DateTime)];

    public static void EnsureTypeAllowedAsRdxValueType(this Type type)
    {
        if (!RdxValueAllowedTypes.Contains(type))
        {
            throw new InvalidOperationException($"Could not create RdxValue with type: {type}");
        }
    }
}