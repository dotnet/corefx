using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    abstract class ConsumableEnumerator<V> : ConsumableForMerging<V>, IEnumerable<V>, IEnumerator<V>, IConsumableInternal
    {
        private readonly int _threadId;
        internal int _state;
        internal V _current;

        protected ConsumableEnumerator()
        {
            _threadId = Environment.CurrentManagedThreadId;
        }

        V IEnumerator<V>.Current => _current;
        object IEnumerator.Current => _current;

        void IEnumerator.Reset() => throw Error.NotSupported();

        public virtual void Dispose()
        {
            _state = int.MaxValue;
            _current = default;
        }

        public override IEnumerator<V> GetEnumerator()
        {
            ConsumableEnumerator<V> enumerator = _state == 0 && _threadId == Environment.CurrentManagedThreadId ? this : Clone();
            enumerator._state = 1;
            return enumerator;
        }

        internal abstract ConsumableEnumerator<V> Clone();

        public abstract bool MoveNext();
    }

    sealed partial class SelectArray<T, U> : ConsumableEnumerator<U>
    {
        internal T[] Underlying { get; }
        internal Func<T, U> Selector { get; }

        int _idx;

        public SelectArray(T[] array, Func<T, U> selector) =>
            (Underlying, Selector) = (array, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.ReadOnlyMemory.Invoke(Underlying, new Links.Select<T, U>(Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new SelectArray<T, U>(Underlying, Selector);

        public override bool MoveNext()
        {
            if (_state != 1 || _idx >= Underlying.Length)
            {
                _current = default;
                return false;
            }

            _current = Selector(Underlying[_idx++]);

            return true;
        }

        public override object TailLink => this;

        public override Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink)
        {
            throw new NotImplementedException();
        }

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new Array<T, U>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new Array<T, V>(Underlying, 0, Underlying.Length, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));
    }

    sealed partial class SelectList<T, U> : ConsumableEnumerator<U>
    {
        internal List<T> Underlying { get; }
        internal Func<T, U> Selector { get; }

        List<T>.Enumerator _enumerator;

        public SelectList(List<T> list, Func<T, U> selector) =>
            (Underlying, Selector) = (list, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.List.Invoke(Underlying, new Links.Select<T, U>(Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new SelectList<T, U>(Underlying, Selector);

        public override bool MoveNext()
        {
            switch (_state)
            {
                case 1:
                    _enumerator = Underlying.GetEnumerator();
                    _state = 2;
                    goto case 2;

                case 2:
                    if (!_enumerator.MoveNext())
                    {
                        _state = int.MaxValue;
                        goto default;
                    }
                    _current = Selector(_enumerator.Current);
                    return true;

                default:
                    _current = default;
                    return false;
            }
        }

        public override object TailLink => this;

        public override Consumable<V1> ReplaceTailLink<Unknown, V1>(Link<Unknown, V1> newLink)
        {
            throw new NotImplementedException();
        }

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new List<T, U>(Underlying, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new List<T, V>(Underlying, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));
    }

    sealed partial class SelectEnumerable<T, U> : ConsumableEnumerator<U>
    {
        internal IEnumerable<T> Underlying { get; }
        internal Func<T, U> Selector { get; }

        IEnumerator<T> _enumerator;

        public SelectEnumerable(IEnumerable<T> enumerable, Func<T, U> selector) =>
            (Underlying, Selector) = (enumerable, selector);

        public override void Consume(Consumer<U> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(Underlying, new Links.Select<T, U>(Selector), consumer);

        internal override ConsumableEnumerator<U> Clone() =>
            new SelectEnumerable<T, U>(Underlying, Selector);

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
                    if (!_enumerator.MoveNext())
                    {
                        _state = int.MaxValue;
                        goto default;
                    }
                    _current = Selector(_enumerator.Current);
                    return true;

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

        public override Consumable<V1> ReplaceTailLink<Unknown, V1>(Link<Unknown, V1> newLink) =>
            throw new NotImplementedException();

        public override Consumable<U> AddTail(Link<U, U> transform) =>
            new Enumerable<T, U>(Underlying, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));

        public override Consumable<V> AddTail<V>(Link<U, V> transform) =>
            new Enumerable<T, V>(Underlying, Links.Composition.Create(new Links.Select<T, U>(Selector), transform));
    }
}
