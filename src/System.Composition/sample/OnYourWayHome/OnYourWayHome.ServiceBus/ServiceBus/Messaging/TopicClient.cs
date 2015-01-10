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
    public sealed class TopicClient : ITopicClient
    {
        private readonly string path;
        private readonly TokenProvider tokenProvider;

        public TopicClient(string path, TokenProvider tokenProvider)
        {
            this.path = path;
            this.tokenProvider = tokenProvider;
        }

        public string Path
        {
            get { return this.path; }
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }
        
        public static IAsyncResult BeginCreateTopic(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            return BeginCreateTopic(path, new TopicDescription(), tokenProvider, callback, state);
        }

        public static IAsyncResult BeginCreateTopic(string path, TopicDescription description, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new CreateEntityAsyncResult<TopicDescription>(path, description, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static TopicClient EndCreateTopic(IAsyncResult result)
        {
            var asyncResult = (CreateEntityAsyncResult<TopicDescription>)result;
            var topicEntity = asyncResult.EndInvoke();

            return new TopicClient(asyncResult.Path, asyncResult.TokenProvider);
        }

        public static IAsyncResult BeginGetTopic(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityAsyncResult<TopicDescription>(path, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static TopicDescription EndGetTopic(IAsyncResult result)
        {
            var asyncResult = (GetEntityAsyncResult<TopicDescription>)result;
            var topicEntity = asyncResult.EndInvoke();

            var topicDescription = topicEntity.Description;
            topicDescription.UpdateFromEntity(topicEntity);

            return topicDescription;
        }

        public static IAsyncResult BeginGetTopics(TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityCollectionAsyncResult<TopicDescription>("$Resources/Topics", tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static IEnumerable<TopicDescription> EndGetTopics(IAsyncResult result)
        {
            var asyncResult = (GetEntityCollectionAsyncResult<TopicDescription>)result;
            var topicEntities = asyncResult.EndInvoke();

            foreach (var topicEntity in topicEntities)
            {
                var topicDescription = topicEntity.Description;
                topicDescription.UpdateFromEntity(topicEntity);

                yield return topicDescription;
            }
        }

        public static IAsyncResult BeginDeleteTopic(string path, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new DeleteEntityAsyncResult(path, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static void EndDeleteTopic(IAsyncResult result)
        {
            var asyncResult = (DeleteEntityAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginGetDescription(AsyncCallback callback, object state)
        {
            return TopicClient.BeginGetTopic(this.path, this.tokenProvider, callback, state);
        }

        public TopicDescription EndGetDescription(IAsyncResult result)
        {
            return TopicClient.EndGetTopic(result);
        }

        public IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback)
        {
            return BeginSend(message, callback, (object)null);
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

        public IAsyncResult BeginAddSubscription(string name, SubscriptionDescription description, AsyncCallback callback, object state)
        {
            return SubscriptionClient.BeginCreateSubscription(this.path, name, description, this.tokenProvider, callback, state);
        }
        
        public SubscriptionClient EndAddSubscription(IAsyncResult result)
        {
            return SubscriptionClient.EndCreateSubscription(result);
        }

        public IAsyncResult BeginGetSubscription(string name, AsyncCallback callback, object state)
        {
            return SubscriptionClient.BeginGetSubscription(this.path, name, this.tokenProvider, callback, state);
        }

        public SubscriptionDescription EndGetSubscription(IAsyncResult result)
        {
            return SubscriptionClient.EndGetSubscription(result);
        }

        public IAsyncResult BeginGetSubscriptions(AsyncCallback callback, object state)
        {
            return SubscriptionClient.BeginGetSubscriptions(this.path, this.tokenProvider, callback, state);
        }

        public IEnumerable<SubscriptionDescription> EndGetSubscriptions(IAsyncResult result)
        {
            return SubscriptionClient.EndGetSubscriptions(result);
        }

        public IAsyncResult BeginRemoveSubscription(string name, AsyncCallback callback, object state)
        {
            return SubscriptionClient.BeginDeleteSubscription(this.path, name, this.tokenProvider, callback, state);
        }

        public void EndRemoveSubscription(IAsyncResult result)
        {
            SubscriptionClient.EndDeleteSubscription(result);
        }
    }
}
