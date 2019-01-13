using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq
{
    abstract class Chain
    {
        public abstract void ChainComplete();
        public abstract void ChainDispose();
    }

    [Flags]
    enum ChainStatus
    {
        /// <summary>
        /// Filter should not be used a flag, rather Flow flag not set
        /// </summary>
        Filter = 0x00,
        Flow = 0x01,
        Stop = 0x02,
    }

    static class ProcessNextResultHelper
    {
        public static bool IsStopped(this ChainStatus result) =>
            (result & ChainStatus.Stop) == ChainStatus.Stop;

        public static bool IsFlowing(this ChainStatus result) =>
            (result & ChainStatus.Flow) == ChainStatus.Flow;
    }

    abstract class Chain<T> : Chain
    {
        public abstract ChainStatus ProcessNext(T input);
    }

    abstract class Link
    {
        protected Link(Links.LinkType linkType) => LinkType = linkType;

        public Links.LinkType LinkType { get; }
    }

    abstract class Link<T, U> : Link
    {
        protected Link(Links.LinkType linkType) : base(linkType) {}

        public abstract Chain<T> Compose(Chain<U> activity);
    }

    abstract class Activity<T, U> : Chain<T>
    {
        private readonly Chain<U> next;

        protected Activity(Chain<U> next) =>
            this.next = next;

        protected ChainStatus Next(U u) =>
            next.ProcessNext(u);

        public override void ChainComplete() => next.ChainComplete();
        public override void ChainDispose() => next.ChainDispose();
    }

    sealed class ChainEnd { private ChainEnd() { } }

    abstract class Consumer<T, R> : Chain<T>
    {
        protected Consumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }

        public override void ChainComplete() { }
        public override void ChainDispose() { }
    }

    internal abstract class Consumable<T> : IEnumerable<T>
    {
        public abstract Result Consume<Result>(Consumer<T, Result> consumer);

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal abstract class ConsumableForAddition<T> : Consumable<T>
    {
        public abstract Consumable<U> AddTail<U>(Link<T, U> transform);
        public abstract Consumable<T> AddTail(Link<T, T> transform);
    }

    abstract class ConsumableForMerging<T> : ConsumableForAddition<T>
    {
        public abstract object TailLink { get; }
        public abstract Consumable<V> ReplaceTailLink<Unknown, V>(Link<Unknown, V> newLink);
    }
}
