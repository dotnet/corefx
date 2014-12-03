namespace System.Linq
{
    internal class AnyFunction<T>
    {
        public static readonly Func<T, bool> Instance = Function;

        private static bool Function(T arg)
        {
            return true;
        }
    }
}