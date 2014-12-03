namespace System.Linq
{
    internal class CastFunction<TSource, TCastTo>
    {
        public static readonly Func<TSource, TCastTo> Instance = Function;

        private static TCastTo Function(TSource arg)
        {
            return (TCastTo)(object)arg;
        }
    }
}