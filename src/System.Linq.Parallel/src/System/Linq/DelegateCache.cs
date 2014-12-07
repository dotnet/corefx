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

        public static readonly Func<T, T> IdentityFunction = IdentityFunctionMethod;

        private static bool AnyFunctionMethod(T arg)
        {
            return true;
        }

        private static T IdentityFunctionMethod(T arg)
        {
            return arg;
        }
    }

    internal static class DelegateCache<T1, T2>
    {
        public static readonly Func<T1, T2> CastFunction = CastFunctionMethod;

        private static T2 CastFunctionMethod(T1 arg)
        {
            return (T2)(object)arg;
        }
    }
}