namespace System.Linq
{
    internal static class DelegateCache
    {
        private static Action s_emptyAction;

        public static Action EmptyAction
        {
            get { return s_emptyAction ?? (s_emptyAction = EmptyActionMethod); }
        }

        private static void EmptyActionMethod()
        {
        }
    }

    internal static class DelegateCache<T>
    {
        private static Func<T, bool> s_anyFunction;
        private static Func<T, T> s_identityFunction;

        public static Func<T, bool> AnyFunction
        {
            get { return s_anyFunction ?? (s_anyFunction = AnyFunctionMethod); }
        }

        public static Func<T, T> IdentityFunction
        {
            get { return s_identityFunction ?? (s_identityFunction = IdentityFunctionMethod); }
        }

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
        private static Func<T1, T2> s_castFunction;
        private static Func<T1, bool> s_compatibleFunction;

        public static Func<T1, T2> CastFunction
        {
            get { return s_castFunction ?? (s_castFunction = CastFunctionMethod); }
        }

        public static Func<T1, bool> CompatibleFunction
        {
            get { return s_compatibleFunction ?? (s_compatibleFunction = CompatibleFunctionMethod); }
        }

        private static T2 CastFunctionMethod(T1 arg)
        {
            return (T2)(object)arg;
        }

        private static bool CompatibleFunctionMethod(T1 arg)
        {
            return arg is T2;
        }
    }
}