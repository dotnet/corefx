// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading.Tasks.Dataflow.Tests
{
    internal sealed class DelegateDisposable : IDisposable
    {
        public Action DisposeDelegate = null;

        void IDisposable.Dispose()
        {
            if (DisposeDelegate != null)
                DisposeDelegate();
        }
    }

    internal sealed class DelegatePropagator<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>
    {
        public delegate TOutput ConsumeMessageFunc(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed);

        public Func<DataflowMessageHeader, TInput, ISourceBlock<TInput>, bool, DataflowMessageStatus> OfferMessageDelegate = null;
        public Func<Task> CompletionDelegate = null;
        public Action CompleteDelegate = null;
        public Action<Exception> FaultDelegate = null;
        public Func<ITargetBlock<TOutput>, DataflowLinkOptions, IDisposable> LinkToDelegate = null;
        public ConsumeMessageFunc ConsumeMessageDelegate = null;
        public Func<DataflowMessageHeader, ITargetBlock<TOutput>, bool> ReserveMessageDelegate = null;
        public Action<DataflowMessageHeader, ITargetBlock<TOutput>> ReleaseMessageDelegate = null;

        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        {
            return OfferMessageDelegate != null ?
                OfferMessageDelegate(messageHeader, messageValue, source, consumeToAccept) :
                DataflowMessageStatus.Accepted;
        }

        public Task Completion 
        {
            get 
            { 
                return CompletionDelegate != null ? 
                    CompletionDelegate() : 
                    null; 
            } 
        }

        public void Complete() 
        {
            if (CompleteDelegate != null) 
                CompleteDelegate(); 
        }

        public void Fault(Exception exception) 
        {
            if (FaultDelegate != null)
                FaultDelegate(exception); 
        }

        public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
        {
            return LinkToDelegate != null ? 
                LinkToDelegate(target, linkOptions) : 
                new DelegateDisposable();
        }

        public TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
        {
            messageConsumed = false;
            return ConsumeMessageDelegate != null ?
                ConsumeMessageDelegate(messageHeader, target, out messageConsumed) :
                default(TOutput);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            return ReserveMessageDelegate != null ?
                ReserveMessageDelegate(messageHeader, target) :
                true;
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            if (ReleaseMessageDelegate != null)
                ReleaseMessageDelegate(messageHeader, target);
        }
    }

    internal sealed class DelegateObserver<T> : IObserver<T>
    {
        public Action<T> OnNextDelegate = null;
        public Action<Exception> OnErrorDelegate = null;
        public Action OnCompletedDelegate = null;

        void IObserver<T>.OnNext(T value) 
        { 
            if (OnNextDelegate != null) 
                OnNextDelegate(value); 
        }
        
        void IObserver<T>.OnError(Exception error) 
        {
            if (OnErrorDelegate != null)
                OnErrorDelegate(error); 
        }

        void IObserver<T>.OnCompleted() 
        {
            if (OnCompletedDelegate != null) 
                OnCompletedDelegate(); 
        }
    }

    internal sealed class DelegateObservable<T> : IObservable<T>
    {
        public Func<IObserver<T>, IDisposable> SubscribeDelegate = null;

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            return SubscribeDelegate != null ? 
                SubscribeDelegate(observer) : 
                null;
        }
    }
}
