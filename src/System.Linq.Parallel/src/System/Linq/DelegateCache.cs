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
    }

    internal static class DelegateCache<T1, T2>
    {
    }
}