namespace Common.Extensions;

public static class AnyExtensions
{
    public static T[] AsList<T>(params T[] elems) => elems;
    
    public static void IfAnyFalse<T>(this IEnumerable<T> elems, Func<T, bool> getField, Action doWhat) => 
        elems.Select(getField).IfAnyFalse(doWhat);

    public static void IfEmpty<T>(this IEnumerable<T> elems, Action func) => func();

    public static void ForEachElement<T>(this T[] array, Action<T> func)
    {
        foreach (var elem in array)
        {
            func(elem);
        }
    }
    public static async Task ForEachElement<T>(this T[] array, Func<T, Task> func)
    {
        foreach (var elem in array)
        {
            await func(elem);
        }
    }
}