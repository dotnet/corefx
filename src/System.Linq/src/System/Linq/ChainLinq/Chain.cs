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
        Filter = 0x00,
        Flow = 0x01,
        Stop = 0x02,
        Consumer = 0x04,

        StoppedConsumer = Stop | Consumer,
    }

    static class ProcessNextResultHelper
    {
        public static bool IsStopped(this ChainStatus result) =>
            (result & ChainStatus.Stop) == ChainStatus.Stop;

        public static bool IsFlowing(this ChainStatus result) =>
            (result & ChainStatus.Flow) == ChainStatus.Flow;

        public static bool IsStoppedConsumer(this ChainStatus result) =>
            (result & ChainStatus.StoppedConsumer) == ChainStatus.StoppedConsumer;
    }

    abstract class Chain<T> : Chain
    {
        public abstract ChainStatus ProcessNext(T input);
    }

    abstract class Chain<T, U> : Chain<T> { }

    internal interface ILink<T, U>
    {
        Chain<T, V> Compose<V>(Chain<U, V> activity);
    }

    abstract class Activity<T, U, V> : Chain<T, V>
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

    abstract class Consumer<T, R> : Chain<T, ChainEnd>
    {
        protected Consumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }

        public override void ChainComplete() { }
        public override void ChainDispose() { }
    }

    internal abstract class Consumable<T>
        : IEnumerable<T>
    {
        public abstract Consumable<U> AddTail<U>(ILink<T, U> transform);

        public abstract Result Consume<Result>(Consumer<T, Result> consumer);

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
