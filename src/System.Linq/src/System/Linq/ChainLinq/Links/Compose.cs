namespace System.Linq.ChainLinq.Links
{
    class Composition<T, U, V> : ILink<T, V>
    {
        private readonly ILink<T, U> _first;
        private readonly ILink<U, V> _second;

        public Composition(ILink<T, U> first, ILink<U, V> second) =>
            (_first, _second) = (first, second);

        public Chain<T, W> Compose<W>(Chain<V, W> next) =>
            _first.Compose(_second.Compose(next));
    }

    static class Composition
    {
        public static ILink<T, V> Create<T, U, V>(ILink<T, U> first, ILink<U, V> second) =>
            new Composition<T, U, V>(first, second);
    }

}
