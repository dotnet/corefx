using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    sealed class Distinct<T> : Link<T, T>
    {
        private readonly IEqualityComparer<T> comparer;

        public Distinct(IEqualityComparer<T> comparer) : base(LinkType.Distinct) =>
            this.comparer = comparer;

        public override Chain<T, V> Compose<V>(Chain<T, V> activity) =>
            new Activity<V>(comparer, activity);

        sealed class Activity<V> : Activity<T, T, V>
        {
            private Set<T> _seen;

            public Activity(IEqualityComparer<T> comparer, Chain<T, V> next) : base(next) =>
                _seen = new Set<T>(comparer);

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

}
