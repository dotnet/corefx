namespace System.Linq
{
    internal static class DelegateCache
    {
        public static readonly Action EmptyAction = EmptyActionMethod;

        private static void EmptyActionMethod()
        {
        }
    }

    internal static class DelegateCache<T>
    {
        public static readonly Func<T, bool> AnyFunction = AnyFunctionMethod;

        private static bool AnyFunctionMethod(T arg)
        {
            return true;
        }
    }

    internal static class DelegateCache<T1, T2>
    {
    }
}