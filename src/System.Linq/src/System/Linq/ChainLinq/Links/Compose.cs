using System.Diagnostics;

namespace System.Linq.ChainLinq.Links
{
    abstract class Composition<T, U> : Link<T, U>
    {
        protected Composition() : base(LinkType.Compose) { }

        public abstract object TailLink { get; }
        public abstract Link<T, V> ReplaceTail<Unknown, V>(Link<Unknown, V> newLink);
    }

    sealed partial class Composition<T, U, V> : Composition<T, V>
    {
        private readonly Link<T, U> _first;
        private readonly Link<U, V> _second;

        public Composition(Link<T, U> first, Link<U, V> second) =>
            (_first, _second) = (first, second);

        public override Chain<T, ChainEnd> Compose(Chain<V, ChainEnd> next) =>
            _first.Compose(_second.Compose(next));

        public override object TailLink => _second;

        public override Link<T, W> ReplaceTail<Unknown, W>(Link<Unknown, W> newLink)
        {
            Debug.Assert(typeof(Unknown) == typeof(U));

            return new Composition<T, U, W>(_first, (Link<U,W>)(object)newLink);
        }
    }

    static class Composition
    {
        public static Link<T, V> Create<T, U, V>(Link<T, U> first, Link<U, V> second)
        {
            if (ReferenceEquals(Identity<T>.Instance, first))
            {
                return (Link<T, V>)(object)second;
            }

            return new Composition<T, U, V>(first, second);
        }
    }

}
