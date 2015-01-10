//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;

    public sealed class BrokeredMessage
    {
        private readonly IDictionary<string, object> messageHeaders;
        private readonly Stream bodyStream;

        private readonly BrokerProperties brokerProperties;

        public BrokeredMessage(object body)
            : this(Serialize(body))
        {
            this.ContentType = "application/xml";
        }

        public BrokeredMessage(Stream bodyStream)
            : this(bodyStream, new Dictionary<string, object>())
        {
        }

        public BrokeredMessage(Stream body, IDictionary<string, object> headers)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            this.bodyStream = body;
            this.messageHeaders = headers;

            this.brokerProperties = new BrokerProperties();
        }

        internal BrokeredMessage(Stream body, WebHeaderCollection headers)
            : this(body, WebHeadersToDictionary(headers))
        {
            if (!string.IsNullOrEmpty(headers["BrokerProperties"]))
            {
                this.brokerProperties = BrokerProperties.Deserialize(headers["BrokerProperties"]);
            }
        }

        public string ContentType { get; set; }

        public string CorrelationId
        {
            get { return this.brokerProperties.CorrelationId; }
            set { this.brokerProperties.CorrelationId = value; }
        }

        public string SessionId
        {
            get { return this.brokerProperties.SessionId; }
            set { this.brokerProperties.SessionId = value; }
        }

        public int DeliveryCount
        {
            get { return this.brokerProperties.DeliveryCount ?? 0; }
        }

        public DateTime LockedUntilUtc
        {
            get { return this.brokerProperties.LockedUntilUtc ?? DateTime.MinValue; }
        }

        public Uri LockLocation
        {
            get
            {
                if (this.messageHeaders.ContainsKey("Location"))
                {
                    var locationString = this.messageHeaders["Location"].ToString();
                    Uri location;

                    if (Uri.TryCreate(locationString, UriKind.Absolute, out location))
                    {
                        return location;
                    }
                }

                return null;
            }
        }

        public Guid LockToken
        {
            get { return this.brokerProperties.LockToken ?? Guid.Empty; }
        }

        public string MessageId
        {
            get { return this.brokerProperties.MessageId; }
        }

        public string Label
        {
            get { return this.brokerProperties.Label; }
            set { this.brokerProperties.Label = value; }
        }

        public string ReplyTo
        {
            get { return this.brokerProperties.ReplyTo; }
            set { this.brokerProperties.ReplyTo = value; }
        }

        public string ReplyToSessionId
        {
            get { return this.brokerProperties.ReplyToSessionId; }
            set { this.brokerProperties.ReplyToSessionId = value; }
        }

        public DateTime SentTimeUtc
        {
            get
            {
                if (this.messageHeaders.ContainsKey("Date"))
                {
                    var dateString = this.messageHeaders["Date"].ToString();
                    DateTime date;

                    if (DateTime.TryParse(dateString, out date))
                    {
                        return date;
                    }
                }

                return DateTime.MinValue;
            }
        }

        public long SequenceNumber
        {
            get { return this.brokerProperties.SequenceNumber ?? -1; }
        }

        public TimeSpan TimeToLive
        {
            get { return this.brokerProperties.TimeToLive ?? TimeSpan.MaxValue; }
            set { this.brokerProperties.TimeToLive = value; }
        }

        public string To
        {
            get { return this.brokerProperties.To; }
            set { this.brokerProperties.To = value; }
        }

        public DateTime ScheduledEnqueueTimeUtc
        {
            get { return this.brokerProperties.ScheduledEnqueueTimeUtc ?? DateTime.MinValue; }
            set { this.brokerProperties.ScheduledEnqueueTimeUtc = value; }
        }

        public Stream BodyStream
        {
            get { return this.bodyStream; }
        }

        public IDictionary<string, object> Headers
        {
            get { return this.messageHeaders; }
        }

        internal BrokerProperties BrokerProperties
        {
            get { return this.brokerProperties; }
        }

        public T GetBody<T>()
        {
            XmlObjectSerializer serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(this.bodyStream);
        }

        public void SetMessageProperty(string name, object value)
        {
            if (value is string)
            {
                this.Headers.Add(name, "\"" + value + "\"");
            }
            else
            {
                this.Headers.Add(name, value);
            }
        }

        internal void UpdateHeaderDictionary()
        {
            if (!this.Headers.ContainsKey("BrokerProperties"))
            {
                this.Headers.Add("BrokerProperties", this.BrokerProperties.Serialize());
            }
            else
            {
                this.Headers["BrokerProperties"] = this.BrokerProperties.Serialize();
            }
        }

        private static Stream Serialize(object body)
        {
            XmlObjectSerializer serializer = new DataContractSerializer(body.GetType());

            MemoryStream stream = new MemoryStream(256);
            serializer.WriteObject(stream, body);
            stream.Flush();
            stream.Position = 0;

            return stream;
        }

        private static IDictionary<string, object> WebHeadersToDictionary(WebHeaderCollection webHeaders)
        {
            if (webHeaders == null)
            {
                throw new ArgumentNullException("webHeaders");
            }

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var headerKey in webHeaders.AllKeys)
            {
                result.Add(headerKey, webHeaders[headerKey]);
            }

            return result;
        }
    }
}
