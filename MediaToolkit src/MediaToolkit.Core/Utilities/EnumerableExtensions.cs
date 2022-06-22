namespace MediaToolkit.Core.Utilities;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        if (action == null) throw new ArgumentNullException("action");

        foreach (var t in collection) action(t);
    }
}