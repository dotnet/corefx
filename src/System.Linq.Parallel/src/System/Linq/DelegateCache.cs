namespace System.Linq
{
    internal static class DelegateCache
    {
        private static Action emptyAction;

        public static Action EmptyAction
        {
            get
            {
                if (emptyAction == null)
                {
                    emptyAction = EmptyActionMethod;
                }

                return emptyAction;
            }
        }

        private static void EmptyActionMethod()
        {
        }
    }

    internal static class DelegateCache<T>
    {
        private static Func<T, bool> anyFunction;
        private static Func<T, T> identityFunction;

        public static Func<T, bool> AnyFunction
        {
            get
            {
                if (anyFunction == null)
                {
                    anyFunction = AnyFunctionMethod;
                }

                return anyFunction;
            }
        }

        public static Func<T, T> IdentityFunction
        {
            get
            {
                if (identityFunction == null)
                {
                    identityFunction = IdentityFunctionMethod;
                }

                return identityFunction;
            }
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
        public static readonly Func<T1, bool> CompatibleFunction = CompatibleFunctionMethod;
        private static Func<T1, T2> castFunction;

        public static Func<T1, T2> CastFunction
        {
            get
            {
                if (castFunction == null)
                {
                    castFunction = CastFunctionMethod;
                }

                return castFunction;
            }
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