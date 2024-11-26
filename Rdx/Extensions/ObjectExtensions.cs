namespace Rdx;

public static class ObjectExtensions
{
    public static void EnsureNotNull(this object? obj, string? name = null)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(name ?? nameof(obj));
        }
    }
}