//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Collections.Generic;
    using OnYourWayHome.AccessControl;  
    using OnYourWayHome.ServiceBus;

    /// <summary>
    /// 
    /// </summary>
    public sealed class QueueClient
    {
        private readonly string path;
        private readonly TokenProvider tokenProvider;

        public QueueClient(string path, TokenProvider tokenProvider)
        {
            this.path = path;
            this.tokenProvider = tokenProvider;

            this.Timeout = TimeSpan.FromSeconds(60);
            this.ReceiveMode = ReceiveMode.ReceiveAndDelete;
        }

        public string Path
        {
            get { return this.path; }
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }

        public TimeSpan Timeout { get; set; }

        public ReceiveMode ReceiveMode { get; set; }

        public static IAsyncResult BeginCreateQueue(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            return BeginCreateQueue(path, new QueueDescription(), tokenProvider, callback, state);
        }

        public static IAsyncResult BeginCreateQueue(string path, QueueDescription description, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new CreateEntityAsyncResult<QueueDescription>(path, description, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static QueueClient EndCreateQueue(IAsyncResult result)
        {
            var asyncResult = (CreateEntityAsyncResult<QueueDescription>)result;
            var queueEntity = asyncResult.EndInvoke();

            return new QueueClient(asyncResult.Path, asyncResult.TokenProvider);
        }

        public static IAsyncResult BeginGetQueue(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityAsyncResult<QueueDescription>(path, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static QueueDescription EndGetQueue(IAsyncResult result)
        {
            var asyncResult = (GetEntityAsyncResult<QueueDescription>)result;
            var queueEntity = asyncResult.EndInvoke();

            var queueDescription = queueEntity.Description;
            queueDescription.UpdateFromEntity(queueEntity);

            return queueDescription;
        }

        public static IAsyncResult BeginGetQueues(TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityCollectionAsyncResult<QueueDescription>("$Resources/Queues", tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static IEnumerable<QueueDescription> EndGetQueues(IAsyncResult result)
        {
            var asyncResult = (GetEntityCollectionAsyncResult<QueueDescription>)result;
            var queueEntities = asyncResult.EndInvoke();

            foreach (var queueEntity in queueEntities)
            {
                var queueDescription = queueEntity.Description;
                queueDescription.UpdateFromEntity(queueEntity);

                yield return queueDescription;
            }
        }

        public static IAsyncResult BeginDeleteQueue(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new DeleteEntityAsyncResult(path, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static void EndDeleteQueue(IAsyncResult result)
        {
            var asyncResult = (DeleteEntityAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginGetDescription(AsyncCallback callback, object state)
        {
            return QueueClient.BeginGetQueue(this.path, this.tokenProvider, callback, state);
        }

        public QueueDescription EndGetDescription(IAsyncResult result)
        {
            return QueueClient.EndGetQueue(result);
        }

        public IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback, object state)
        {
            var asyncResult = new SendMessageAsyncResult(this.path, message, this.tokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndSend(IAsyncResult result)
        {
            var asyncResult = (SendMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.Timeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.BeginReceive(timeout, this.ReceiveMode, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, ReceiveMode receiveMode, AsyncCallback callback, object state)
        {
            ServiceBusRequestAsyncResult<BrokeredMessage> asyncResult;
            if (receiveMode == ReceiveMode.PeekLock)
            {
                asyncResult = new PeekLockMessageAsyncResult(this.path, timeout, this.tokenProvider);
            }
            else
            {
                asyncResult = new ReceiveAndDeleteMessageAsyncResult(this.path, timeout, this.tokenProvider);
            }

            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public BrokeredMessage EndReceive(IAsyncResult result)
        {
            var asyncResult = (ServiceBusRequestAsyncResult<BrokeredMessage>)result;
            return asyncResult.EndInvoke();
        }

        public IAsyncResult BeginAbandon(BrokeredMessage message, AsyncCallback callback, object state)
        {
            var asyncResult = new AbandonMessageAsyncResult(message, this.tokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndAbandon(IAsyncResult result)
        {
            var asyncResult = (AbandonMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginComplete(BrokeredMessage message, AsyncCallback callback, object state)
        {
            var asyncResult = new CompleteMessageAsyncResult(message, this.tokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndComplete(IAsyncResult result)
        {
            var asyncResult = (CompleteMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }
    }
}
