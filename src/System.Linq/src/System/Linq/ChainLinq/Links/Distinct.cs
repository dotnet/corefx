using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    sealed class Distinct<T> : Link<T, T>
    {
        private readonly IEqualityComparer<T> comparer;

        public Distinct(IEqualityComparer<T> comparer) : base(LinkType.Distinct) =>
            this.comparer = comparer;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(comparer, activity);

        sealed class Activity : Activity<T, T>
        {
            private Set<T> _seen;

            public Activity(IEqualityComparer<T> comparer, Chain<T> next) : base(next) =>
                _seen = new Set<T>(comparer);

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

    sealed class DistinctDefaultComparer<T> : Link<T, T>
    {
        public static readonly Link<T, T> Instance = new DistinctDefaultComparer<T>();

        private DistinctDefaultComparer() : base(LinkType.Distinct) { }

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(activity);

        sealed class Activity : Activity<T, T>
        {
            private SetDefaultComparer<T> _seen;

            public Activity(Chain<T> next) : base(next) =>
                _seen = new SetDefaultComparer<T>();

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

}
