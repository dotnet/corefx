using System.Diagnostics;

namespace System.Linq.ChainLinq.Links
{
    abstract class Composition<T, U> : ILink<T, U>
    {
        public abstract Chain<T, V> Compose<V>(Chain<U, V> activity);

        public abstract object TailLink { get; }
        public abstract ILink<T, V> ReplaceTail<Unknown, V>(ILink<Unknown, V> newLink);
    }

    class Composition<T, U, V> : Composition<T, V>
    {
        private readonly ILink<T, U> _first;
        private readonly ILink<U, V> _second;

        public Composition(ILink<T, U> first, ILink<U, V> second) =>
            (_first, _second) = (first, second);

        public override Chain<T, W> Compose<W>(Chain<V, W> next) =>
            _first.Compose(_second.Compose(next));

        public override object TailLink => _second;

        public override ILink<T, W> ReplaceTail<Unknown, W>(ILink<Unknown, W> newLink)
        {
            Debug.Assert(typeof(Unknown) == typeof(U));

            return new Composition<T, U, W>(_first, (ILink<U,W>)newLink);
        }
    }

    static class Composition
    {
        public static ILink<T, V> Create<T, U, V>(ILink<T, U> first, ILink<U, V> second)
        {
            if (ReferenceEquals(Identity<T>.Instance, first))
            {
                return (ILink<T, V>)second;
            }

            return new Composition<T, U, V>(first, second);
        }
    }

}
