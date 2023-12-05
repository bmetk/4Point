namespace Common.Extensions;

public static class BooleanExtensions
{
    public static void IfAnyFalse(this IEnumerable<bool> elems, Action doWhat)
    {
        if (elems.Any(x => x == false)) doWhat();
    }
}