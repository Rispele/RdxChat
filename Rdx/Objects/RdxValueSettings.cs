namespace Rdx.Objects;

public static class RdxValueSettings
{
    public static readonly Type[] RdxValueAllowedTypes = [typeof(string), typeof(int), typeof(long), typeof(double), typeof(bool)];

    public static void EnsureTypeAllowedAsRdxValueType(this Type type)
    {
        if (!RdxValueAllowedTypes.Contains(type))
        {
            throw new InvalidOperationException($"Could not create RdxValue with type: {type}");
        }
    }
}