namespace Rdx;

public static class EnumerableExtensions
{
    public static IEnumerable<T> AsEnumerable<T>(this T value)
    {
        yield return value;
    }
}