using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    sealed class Distinct<T> : Link<T, T>
    {
        private readonly IEqualityComparer<T> comparer;

        public Distinct(IEqualityComparer<T> comparer) : base(LinkType.Distinct) =>
            this.comparer = comparer;

        public override Chain<T, ChainEnd> Compose(Chain<T, ChainEnd> activity) =>
            new Activity(comparer, activity);

        sealed class Activity : Activity<T, T, ChainEnd>
        {
            private Set<T> _seen;

            public Activity(IEqualityComparer<T> comparer, Chain<T, ChainEnd> next) : base(next) =>
                _seen = new Set<T>(comparer);

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

}
