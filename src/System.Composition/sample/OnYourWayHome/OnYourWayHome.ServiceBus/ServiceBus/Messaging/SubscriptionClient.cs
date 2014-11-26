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
    public sealed class SubscriptionClient : ISubscriptionClient
    {
        private readonly string topicPath;
        private readonly string name;
        private readonly TokenProvider tokenProvider;

        public SubscriptionClient(string topicPath, string name, TokenProvider tokenProvider)
        {
            this.topicPath = topicPath; 
            this.name = name;
            this.tokenProvider = tokenProvider;

            this.Timeout = TimeSpan.FromSeconds(60);
            this.ReceiveMode = ReceiveMode.ReceiveAndDelete;
        }

        public string TopicPath
        {
            get { return this.topicPath; }
        }
                
        public string Name
        {
            get { return this.name; }
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }

        public TimeSpan Timeout { get; set; }

        public ReceiveMode ReceiveMode { get; set; }

        public static IAsyncResult BeginCreateSubscription(string topicPath, string name, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            return BeginCreateSubscription(topicPath, name, new SubscriptionDescription(), tokenProvider, callback, state);
        }

        public static IAsyncResult BeginCreateSubscription(string topicPath, string name, SubscriptionDescription description, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            description.TopicPath = topicPath;
            description.Name = name;

            var result = new CreateEntityAsyncResult<SubscriptionDescription>(topicPath + "/Subscriptions/" + name, description, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static SubscriptionClient EndCreateSubscription(IAsyncResult result)
        {
            var asyncResult = (CreateEntityAsyncResult<SubscriptionDescription>)result;
            var subscriptionEntity = asyncResult.EndInvoke();

            return new SubscriptionClient(asyncResult.EntityDescription.TopicPath, asyncResult.EntityDescription.Name, asyncResult.TokenProvider);
        }

        public static IAsyncResult BeginGetSubscription(string topicPath, string name, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityAsyncResult<SubscriptionDescription>(topicPath + "/Subscriptions/" + name, tokenProvider) { TopicPath = topicPath };
            result.BeginInvoke(callback, state);

            return result;
        }

        public static SubscriptionDescription EndGetSubscription(IAsyncResult result)
        {
            var asyncResult = (GetEntityAsyncResult<SubscriptionDescription>)result;
            var subscriptionEntity = asyncResult.EndInvoke();

            var subscriptionDescription = subscriptionEntity.Description;
            subscriptionDescription.UpdateFromEntity(asyncResult.TopicPath, subscriptionEntity);

            return subscriptionDescription;
        }

        public static IAsyncResult BeginGetSubscriptions(string topicPath, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new GetEntityCollectionAsyncResult<SubscriptionDescription>(topicPath + "/Subscriptions", tokenProvider) { TopicPath = topicPath };
            result.BeginInvoke(callback, state);

            return result;
        }

        public static IEnumerable<SubscriptionDescription> EndGetSubscriptions(IAsyncResult result)
        {
            var asyncResult = (GetEntityCollectionAsyncResult<SubscriptionDescription>)result;
            var subscriptionEntities = asyncResult.EndInvoke();

            foreach (var subscriptionEntity in subscriptionEntities)
            {
                var subscriptionDescription = subscriptionEntity.Description;
                subscriptionDescription.UpdateFromEntity(asyncResult.TopicPath, subscriptionEntity);

                yield return subscriptionDescription;
            }
        }

        public static IAsyncResult BeginDeleteSubscription(string topicPath, string name, TokenProvider tokenProvider, AsyncCallback callback, object state)
        {
            var result = new DeleteEntityAsyncResult(topicPath + "/Subscriptions/" + name, tokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public static void EndDeleteSubscription(IAsyncResult result)
        {
            var asyncResult = (DeleteEntityAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginGetDescription(AsyncCallback callback, object state)
        {
            return SubscriptionClient.BeginGetSubscription(this.topicPath, this.name, this.tokenProvider, callback, state);
        }

        public SubscriptionDescription EndGetDescription(IAsyncResult result)
        {
            return SubscriptionClient.EndGetSubscription(result);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback)
        {
            return BeginReceive(this.Timeout, callback, (object)null);
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
                asyncResult = new PeekLockMessageAsyncResult(this.topicPath + "/Subscriptions/" + this.name, timeout, this.tokenProvider);
            }
            else
            {
                asyncResult = new ReceiveAndDeleteMessageAsyncResult(this.topicPath + "/Subscriptions/" + this.name, timeout, this.tokenProvider);
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

        public IAsyncResult BeginAddRule(string name, RuleDescription description, AsyncCallback callback, object state)
        {
            var result = new CreateEntityAsyncResult<RuleDescription>(this.topicPath + "/Subscriptions/" + this.name + "/Rules/" + name, description, this.TokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public RuleDescription EndAddRule(IAsyncResult result)
        {
            var asyncResult = (CreateEntityAsyncResult<RuleDescription>)result;
            var ruleEntity = asyncResult.EndInvoke();

            var ruleDescription = ruleEntity.Description;
            ruleDescription.UpdateFromEntity(this.topicPath, this.name, ruleEntity);

            return ruleDescription;
        }

        public IAsyncResult BeginGetRule(string name, AsyncCallback callback, object state)
        {
            var result = new GetEntityAsyncResult<RuleDescription>(this.topicPath + "/Subscriptions/" + this.name + "/Rules/" + name, this.TokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public RuleDescription EndGetRule(IAsyncResult result)
        {
            var asyncResult = (GetEntityAsyncResult<RuleDescription>)result;
            var ruleEntity = asyncResult.EndInvoke();

            var ruleDescription = ruleEntity.Description;
            ruleDescription.UpdateFromEntity(this.TopicPath, this.Name, ruleEntity);

            return ruleDescription;
        }

        public IAsyncResult BeginGetRules(AsyncCallback callback, object state)
        {
            var result = new GetEntityCollectionAsyncResult<RuleDescription>(this.topicPath + "/Subscriptions/" + this.name + "/Rules", this.TokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public IEnumerable<RuleDescription> EndGetRules(IAsyncResult result)
        {
            var asyncResult = (GetEntityCollectionAsyncResult<RuleDescription>)result;
            var ruleEntities = asyncResult.EndInvoke();

            foreach (var ruleEntity in ruleEntities)
            {
                var ruleDescription = ruleEntity.Description;
                ruleDescription.UpdateFromEntity(this.TopicPath, this.Name, ruleEntity);

                yield return ruleDescription;
            }
        }

        public IAsyncResult BeginRemoveRule(string name, AsyncCallback callback, object state)
        {
            var result = new DeleteEntityAsyncResult(this.topicPath + "/Subscriptions/" + this.name + "/Rules/" + name, this.TokenProvider);
            result.BeginInvoke(callback, state);

            return result;
        }

        public void EndRemoveRule(IAsyncResult result)
        {
            var asyncResult = (DeleteEntityAsyncResult)result;
            asyncResult.EndInvoke();
        }
    }
}
