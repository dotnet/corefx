namespace System.Linq
{
    internal static class DelegateCache
    {
        private static Action s_emptyAction;

        public static Action EmptyAction
        {
            get
            {
                if (s_emptyAction == null)
                {
                    s_emptyAction = EmptyActionMethod;
                }

                return s_emptyAction;
            }
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
            get
            {
                if (s_anyFunction == null)
                {
                    s_anyFunction = AnyFunctionMethod;
                }

                return s_anyFunction;
            }
        }

        public static Func<T, T> IdentityFunction
        {
            get
            {
                if (s_identityFunction == null)
                {
                    s_identityFunction = IdentityFunctionMethod;
                }

                return s_identityFunction;
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
        private static Func<T1, T2> s_castFunction;
        private static Func<T1, bool> s_compatibleFunction;

        public static Func<T1, T2> CastFunction
        {
            get
            {
                if (s_castFunction == null)
                {
                    s_castFunction = CastFunctionMethod;
                }

                return s_castFunction;
            }
        }

        public static Func<T1, bool> CompatibleFunction
        {
            get
            {
                if (s_compatibleFunction == null)
                {
                    s_compatibleFunction = CompatibleFunctionMethod;
                }

                return s_compatibleFunction;
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