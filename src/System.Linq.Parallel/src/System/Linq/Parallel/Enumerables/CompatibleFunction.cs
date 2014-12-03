namespace System.Linq
{
    internal class CompatibleFunction<TSource, TCastTo>
    {
        public static readonly Func<TSource, bool> Instance = Function;

        private static bool Function(TSource arg)
        {
            return arg is TCastTo;
        }
    }
}