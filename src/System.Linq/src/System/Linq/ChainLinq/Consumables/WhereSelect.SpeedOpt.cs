using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class WhereSelectArray<T, U> : ConsumableEnumerator<U>
    {
        internal T[] Underlying { get; }
        internal Func<T, bool> Predicate { get; }
        internal Func<T, U> Selector { get; }

        int _idx;

        public WhereSelectArray(T[] array, Func<T, bool> predicate, Func<T, U> selector) =>
            (Underlying, Predicate, Selector) = (array, predicate, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.Array.Invoke(Underlying, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new WhereSelectArray<T, U>(Underlying, Predicate, Selector);

        public override bool MoveNext()
        {
            if (_state != 1)
                return false;

            while (_idx < Underlying.Length)
            {
                var current = Underlying[_idx++];
                if (Predicate(current))
                {
                    _current = Selector(current);
                    return true;
                }
            }

            _current = default;
            return false;
        }

        public override object TailLink => this;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink) =>
            throw new NotImplementedException();

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new Array<T, U>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new Array<T, V>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));
    }

    sealed partial class WhereSelectList<T, U> : ConsumableEnumerator<U>
    {
        internal List<T> Underlying { get; }
        internal Func<T, bool> Predicate { get; }
        internal Func<T, U> Selector { get; }

        List<T>.Enumerator _enumerator;

        public WhereSelectList(List<T> list, Func<T, bool> predicate, Func<T, U> selector) =>
            (Underlying, Predicate, Selector) = (list, predicate, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.List.Invoke(Underlying, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new WhereSelectList<T, U>(Underlying, Predicate, Selector);

        public override bool MoveNext()
        {
            switch (_state)
            {
                case 1:
                    _enumerator = Underlying.GetEnumerator();
                    _state = 2;
                    goto case 2;

                case 2:
                    while (_enumerator.MoveNext())
                    {
                        var current = _enumerator.Current;
                        if (Predicate(current))
                        {
                            _current = Selector(current);
                            return true;
                        }
                    }
                    _state = int.MaxValue;
                    goto default;

                default:
                    _current = default;
                    return false;
            }
        }

        public override object TailLink => this;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink) =>
            throw new NotImplementedException();

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new List<T, U>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new List<T, V>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));
    }

    sealed partial class WhereSelectEnumerable<T, U> : ConsumableEnumerator<U>
    {
        internal IEnumerable<T> Underlying { get; }
        internal Func<T, bool> Predicate { get; }
        internal Func<T, U> Selector { get; }

        IEnumerator<T> _enumerator;

        public WhereSelectEnumerable(IEnumerable<T> enumerable, Func<T, bool> predicate, Func<T, U> selector) =>
            (Underlying, Predicate, Selector) = (enumerable, predicate, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(Underlying, new Links.WhereSelect<T, U>(Predicate, Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new WhereSelectEnumerable<T, U>(Underlying, Predicate, Selector);

        public override void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
            base.Dispose();
        }

        public override bool MoveNext()
        {
            switch (_state)
            {
                case 1:
                    _enumerator = Underlying.GetEnumerator();
                    _state = 2;
                    goto case 2;

                case 2:
                    while (_enumerator.MoveNext())
                    {
                        var current = _enumerator.Current;
                        if (Predicate(current))
                        {
                            _current = Selector(current);
                            return true;
                        }
                    }
                    _state = int.MaxValue;
                    goto default;

                default:
                    _current = default;
                    if (_enumerator != null)
                    {
                        _enumerator.Dispose();
                        _enumerator = null;
                    }
                    return false;
            }
        }

        public override object TailLink => this;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink) =>
            throw new NotImplementedException();

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new Enumerable<T, U>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new Enumerable<T, V>(Underlying, Links.Composition.Create(new Links.WhereSelect<T, U>(Predicate, Selector), transform));
    }
}
