namespace System.Linq
{
    internal class IdentityFunction<T>
    {
        public static readonly Func<T, T> Instance = Function;

        private static T Function(T arg)
        {
            return arg;
        }
    }
}