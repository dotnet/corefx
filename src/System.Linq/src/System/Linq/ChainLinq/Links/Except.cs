using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    sealed class Except<T> : Link<T, T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly IEnumerable<T> _second;

        public Except(IEqualityComparer<T> comparer, IEnumerable<T> second) : base(LinkType.Except) =>
            (_comparer, _second) = (comparer, second);

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(_comparer, _second, activity);

        sealed class Activity : Activity<T, T>
        {
            class PopulateSet : Consumer<T>
            {
                private Set<T> _set;

                public PopulateSet(Set<T> set) => _set = set;

                public override ChainStatus ProcessNext(T input)
                {
                    _set.Add(input);
                    return ChainStatus.Flow;
                }
            }

            private Set<T> _seen;

            public Activity(IEqualityComparer<T> comparer, IEnumerable<T> second, Chain<T> next) : base(next)
            {
                _seen = new Set<T>(comparer);
                if (second is Consumable<T> c)
                {
                    c.Consume(new PopulateSet(_seen));
                }
                else
                {
                    foreach (var element in second)
                        _seen.Add(element);
                }
            }

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

    sealed class ExceptDefaultComparer<T> : Link<T, T>
    {
        private readonly IEnumerable<T> _second;

        public ExceptDefaultComparer(IEnumerable<T> second) : base(LinkType.Except) =>
            _second = second;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(_second, activity);

        sealed class Activity : Activity<T, T>
        {
            class PopulateSet : Consumer<T>
            {
                private SetDefaultComparer<T> _set;

                public PopulateSet(SetDefaultComparer<T> set) => _set = set;

                public override ChainStatus ProcessNext(T input)
                {
                    _set.Add(input);
                    return ChainStatus.Flow;
                }
            }

            private SetDefaultComparer<T> _seen;

            public Activity(IEnumerable<T> second, Chain<T> next) : base(next)
            {
                _seen = new SetDefaultComparer<T>();
                if (second is Consumable<T> c)
                {
                    c.Consume(new PopulateSet(_seen));
                }
                else
                {
                    foreach (var element in second)
                        _seen.Add(element);
                }
            }

            public override ChainStatus ProcessNext(T input) =>
                _seen.Add(input) ? Next(input) : ChainStatus.Filter;
        }
    }

}
