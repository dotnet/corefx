namespace System.Linq.ChainLinq.Links
{
    partial class Identity<T> : ILink<T, T>
    {
        public static ILink<T, T> Instance { get; } = new Identity<T>();

        public Chain<T, V> Compose<V>(Chain<T, V> next) => next;
    }
}
